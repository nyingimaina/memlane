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
        private readonly IEnumerable<IStorageProvider> _storageProviders;
        private readonly ISyncEngine _syncEngine;
        private readonly IHubContext<JobHub> _hubContext;
        private readonly ILogger<BackupJobOrchestrator> _logger;

        public BackupJobOrchestrator(
            IEnumerable<IBackupProvider> backupProviders,
            IEnumerable<IStorageProvider> storageProviders,
            ISyncEngine syncEngine,
            IHubContext<JobHub> hubContext,
            ILogger<BackupJobOrchestrator> logger)
        {
            _backupProviders = backupProviders;
            _storageProviders = storageProviders;
            _syncEngine = syncEngine;
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

            _logger.LogInformation("Starting backup job {JobId}: {JobName}", job.Id, job.Name);
            await SendUpdateAsync(job.Id, "Started", "Starting backup job orchestration...", 0);

            // 1. File Synchronization (if applicable) - WE DO THIS FIRST TO DETECT CHANGES
            SyncResult? syncResult = null;
            if (!string.IsNullOrEmpty(config.SourceDirectory) && !string.IsNullOrEmpty(config.TargetDirectory))
            {
                await SendUpdateAsync(job.Id, "FileSync", "Starting file synchronization...", 10);
                _logger.LogInformation("Starting file synchronization from {Source} to {Target}...", config.SourceDirectory, config.TargetDirectory);
                syncResult = await _syncEngine.SyncAsync(config.SourceDirectory, config.TargetDirectory);
                _logger.LogInformation("File synchronization complete. {FilesSynced} files synced.", syncResult.FilesSynced);
                await SendUpdateAsync(job.Id, "FileSync", $"File synchronization complete. {syncResult.FilesSynced} files synced.", 40);
            }

            // CHECK FOR CHANGES AND SKIP IF NECESSARY
            if (config.SkipIfNoChanges && syncResult != null && !syncResult.ChangesDetected)
            {
                _logger.LogInformation("No changes detected for job {JobId}. Skipping database backup and compression.", job.Id);
                await SendUpdateAsync(job.Id, "Skipped", "No changes detected. Skipping backup steps.", 100);
                return JobExecutionResult.Skipped;
            }

            // 2. Database Backup (if applicable)
            string? backupFilePath = null;
            if (!string.IsNullOrEmpty(config.DbConnectionString) && !string.IsNullOrEmpty(config.DbProvider))
            {
                var provider = _backupProviders.FirstOrDefault(p => p.ProviderName.Equals(config.DbProvider, StringComparison.OrdinalIgnoreCase));
                if (provider == null)
                {
                    throw new Exception($"Backup provider '{config.DbProvider}' not found.");
                }

                await SendUpdateAsync(job.Id, "DatabaseBackup", $"Creating database backup using {provider.ProviderName}...", 50);
                _logger.LogInformation("Creating database backup using {Provider}...", provider.ProviderName);
                var tempDir = Path.Combine(Path.GetTempPath(), "Memlane_Temp_" + job.Id);
                if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
                
                backupFilePath = await provider.CreateBackupAsync(config.DbConnectionString, tempDir);
                _logger.LogInformation("Database backup created at {Path}", backupFilePath);
                await SendUpdateAsync(job.Id, "DatabaseBackup", "Database backup complete.", 70);
            }

            // 3. Compression (if enabled)
            if (config.EnableCompression && !string.IsNullOrEmpty(config.TargetDirectory) && !string.IsNullOrEmpty(config.ArchiveFileName))
            {
                var archivePath = Path.Combine(config.TargetDirectory, config.ArchiveFileName);
                await SendUpdateAsync(job.Id, "Compression", $"Compressing backup to {config.ArchiveFileName}...", 80);
                _logger.LogInformation("Compressing backup to {ArchivePath}...", archivePath);
                await CompressionUtility.CompressAsync(config.TargetDirectory, archivePath);
                _logger.LogInformation("Compression complete.");
                await SendUpdateAsync(job.Id, "Compression", "Compression complete.", 95);
            }

            await SendUpdateAsync(job.Id, "Completed", "Backup job finished successfully.", 100);
            _logger.LogInformation("Backup job {JobId} finished successfully.", job.Id);
            return JobExecutionResult.Completed;
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
        public string? DbProvider { get; set; } // e.g., "SQL Server", "MariaDB"
        public string? DbConnectionString { get; set; }
        public string? SourceDirectory { get; set; }
        public string? TargetDirectory { get; set; }
        public bool EnableCompression { get; set; }
        public string? ArchiveFileName { get; set; }
        public bool SkipIfNoChanges { get; set; } = true;
    }
}