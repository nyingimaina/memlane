using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Memlane.Api.Hubs;
using Memlane.Api.Models;
using Memlane.Api.Providers;

namespace Memlane.Api.Services
{
    public enum JobExecutionResult
    {
        Completed,
        Skipped,
        Failed
    }

    public interface IJobOrchestrator
    {
        Task<JobExecutionResult> ExecuteJobAsync(JobMetadata job, CancellationToken stoppingToken);
    }

    public class BackupJobOrchestrator : IJobOrchestrator
    {
        private readonly IEnumerable<IBackupProvider> _backupProviders;
        private readonly IStorageProviderFactory _storageProviderFactory;
        private readonly ISyncEngine _syncEngine;
        private readonly IRetentionManager _retentionManager;
        private readonly IHubContext<JobHub> _hubContext;
        private readonly ILogger<BackupJobOrchestrator> _logger;

        public BackupJobOrchestrator(
            IEnumerable<IBackupProvider> backupProviders,
            IStorageProviderFactory storageProviderFactory,
            ISyncEngine syncEngine,
            IRetentionManager retentionManager,
            IHubContext<JobHub> hubContext,
            ILogger<BackupJobOrchestrator> logger)
        {
            _backupProviders = backupProviders;
            _storageProviderFactory = storageProviderFactory;
            _syncEngine = syncEngine;
            _retentionManager = retentionManager;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<JobExecutionResult> ExecuteJobAsync(JobMetadata job, CancellationToken stoppingToken)
        {
            if (string.IsNullOrEmpty(job.ConfigurationJson))
            {
                throw new ArgumentException($"Job {job.Id} has no configuration.");
            }

            var config = JsonSerializer.Deserialize<BackupJobConfiguration>(job.ConfigurationJson);
            if (config == null)
            {
                throw new ArgumentException($"Failed to deserialize configuration for job {job.Id}.");
            }

            _logger.LogInformation("Starting full pipeline for job {JobId}: {JobName}", job.Id, job.Name);
            await SendUpdateAsync(job.Id, "Started", "Initializing backup pipeline...", 0);

            var tempDir = Path.Combine(Path.GetTempPath(), "Memlane_Pipeline_" + job.Id);
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            Directory.CreateDirectory(tempDir);

            try {
                // 1. Database Backup (Output to Temp)
                string? dbBackupFile = null;
                if (!string.IsNullOrEmpty(config.DbConnectionString) && !string.IsNullOrEmpty(config.DbProvider) && config.DbProvider != "None")
                {
                    var provider = _backupProviders.FirstOrDefault(p => p.ProviderName.Equals(config.DbProvider, StringComparison.OrdinalIgnoreCase));
                    if (provider == null) throw new Exception($"Database provider '{config.DbProvider}' not found.");

                    await SendUpdateAsync(job.Id, "DatabaseBackup", $"Backing up database using {provider.ProviderName}...", 10);
                    dbBackupFile = await provider.CreateBackupAsync(config.DbConnectionString, tempDir);
                    _logger.LogInformation("Database backup ready: {Path}", dbBackupFile);
                }

                // 2. File Synchronization / Hashing (Source to Temp)
                SyncResult? syncResult = null;
                if (!string.IsNullOrEmpty(config.SourceDirectory))
                {
                    await SendUpdateAsync(job.Id, "FileSync", "Checking for file changes...", 30);
                    syncResult = await _syncEngine.SyncAsync(config.SourceDirectory, tempDir);
                    _logger.LogInformation("Sync complete. Changes detected: {Changes}", syncResult.ChangesDetected);
                }

                // 3. Skip Check (If no DB backup and no file changes)
                bool hasChanges = (dbBackupFile != null) || (syncResult != null && syncResult.ChangesDetected);
                if (hasChanges == false && config.SkipIfNoChanges)
                {
                    _logger.LogInformation("No changes detected for job {JobId}. Pipeline halted.", job.Id);
                    await SendUpdateAsync(job.Id, "Skipped", "No data changes detected. Skipping storage steps.", 100);
                    return JobExecutionResult.Skipped;
                }

                // 4. Compression (Temp to Temp Archive)
                string artifactPath = tempDir;
                if (config.EnableCompression && !string.IsNullOrEmpty(config.ArchiveFileName))
                {
                    var archivePath = Path.Combine(Path.GetTempPath(), config.ArchiveFileName);
                    await SendUpdateAsync(job.Id, "Compression", "Compressing artifacts...", 70);
                    await CompressionUtility.CompressAsync(tempDir, archivePath);
                    artifactPath = archivePath;
                    _logger.LogInformation("Compression ready: {Path}", artifactPath);
                }

                // 5. Storage Provider (Resolved via Factory)
                if (!string.IsNullOrEmpty(config.StorageProvider) && !string.IsNullOrEmpty(config.TargetDestination))
                {
                    var storage = _storageProviderFactory.GetProvider(config.StorageProvider);
                    
                    await SendUpdateAsync(job.Id, "Storage", $"Transferring to {storage.ProviderName}...", 90);
                    await storage.SaveAsync(artifactPath, config.TargetDestination);
                    _logger.LogInformation("Transfer to storage complete.");

                    // 6. Backup Rotation (Pruning)
                    if (config.RetentionCount > 0 && (config.StorageProvider == "Local" || config.StorageProvider == "Folder"))
                    {
                        await SendUpdateAsync(job.Id, "Rotation", "Pruning old backups...", 95);
                        await _retentionManager.RotateBackupsAsync(config.TargetDestination, config.RetentionCount);
                    }

                    // If it was a compressed file in Temp, clean it up
                    if (artifactPath != tempDir && File.Exists(artifactPath)) File.Delete(artifactPath);
                }

                await SendUpdateAsync(job.Id, "Completed", "Pipeline finished successfully.", 100);
                return JobExecutionResult.Completed;

            } finally {
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
        }

        private async Task SendUpdateAsync(int jobId, string status, string message, int progress)
        {
            var update = new JobStatusUpdate(jobId, status, message, progress);
            await _hubContext.Clients.Group($"Job_{jobId}").SendAsync("ReceiveStatusUpdate", update);
            await _hubContext.Clients.All.SendAsync("ReceiveGlobalStatusUpdate", update);
        }
    }

    public class BackupJobConfiguration
    {
        public string? DbProvider { get; set; }
        public string? DbConnectionString { get; set; }
        public string? SourceDirectory { get; set; }
        public string? StorageProvider { get; set; } 
        public string? TargetDestination { get; set; }
        public bool EnableCompression { get; set; }
        public string? ArchiveFileName { get; set; }
        public bool SkipIfNoChanges { get; set; } = true;
        public int RetentionCount { get; set; }
    }
}