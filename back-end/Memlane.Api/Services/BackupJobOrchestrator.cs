using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Memlane.Api.Hubs;
using Memlane.Api.Infrastructure;
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
        private readonly IFilenameGenerator _filenameGenerator;
        private readonly ICompressionProvider _compressionProvider;
        private readonly IJobRepository _repository;
        private readonly IHubContext<JobHub> _hubContext;
        private readonly ILogger<BackupJobOrchestrator> _logger;

        public BackupJobOrchestrator(
            IEnumerable<IBackupProvider> backupProviders,
            IStorageProviderFactory storageProviderFactory,
            ISyncEngine syncEngine,
            IRetentionManager retentionManager,
            IFilenameGenerator filenameGenerator,
            ICompressionProvider compressionProvider,
            IJobRepository repository,
            IHubContext<JobHub> hubContext,
            ILogger<BackupJobOrchestrator> logger)
        {
            _backupProviders = backupProviders ?? Array.Empty<IBackupProvider>();
            _storageProviderFactory = storageProviderFactory;
            _syncEngine = syncEngine;
            _retentionManager = retentionManager;
            _filenameGenerator = filenameGenerator;
            _compressionProvider = compressionProvider;
            _repository = repository;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<JobExecutionResult> ExecuteJobAsync(JobMetadata job, CancellationToken stoppingToken)
        {
            if (string.IsNullOrEmpty(job.ConfigurationJson))
            {
                throw new ArgumentException($"Job {job.Id} has no configuration.");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var config = JsonSerializer.Deserialize<BackupJobConfiguration>(job.ConfigurationJson, options);
            
            if (config == null)
            {
                throw new ArgumentException($"Failed to deserialize configuration for job {job.Id}.");
            }

            string CleanPath(string? p) => p?.Trim(' ', '"', '\'') ?? "";
            string sourceDir = CleanPath(config.SourceDirectory);
            if (!string.IsNullOrEmpty(sourceDir) && !Path.IsPathRooted(sourceDir)) sourceDir = Path.GetFullPath(sourceDir);

            string targetDest = CleanPath(config.TargetDestination);
            if (!string.IsNullOrEmpty(targetDest) && !Path.IsPathRooted(targetDest)) targetDest = Path.GetFullPath(targetDest);

            // Create Run Record
            var run = new JobRun {
                JobId = job.Id,
                StartTime = DateTime.UtcNow,
                Status = JobStatus.InProgress,
                Logs = ""
            };
            run.Id = await _repository.AddRunAsync(run);

            var runLogger = new RunLogger(run, _repository, _hubContext, _logger);
            await runLogger.LogAsync($"--- Starting Pipeline: {job.Name} ---");
            await runLogger.LogAsync($"[Config] Source: '{sourceDir}'");
            await runLogger.LogAsync($"[Config] Destination: '{targetDest}'");
            await runLogger.LogAsync($"[Config] SkipIfNoChanges: {config.SkipIfNoChanges}");

            var syncWorkspace = Path.Combine(Path.GetTempPath(), "Memlane", "Sync", job.Id.ToString());
            var artifactWorkspace = Path.Combine(Path.GetTempPath(), "Memlane", "Artifacts", job.Id.ToString(), Guid.NewGuid().ToString().Substring(0, 8));

            if (!Directory.Exists(syncWorkspace)) Directory.CreateDirectory(syncWorkspace);
            if (!Directory.Exists(artifactWorkspace)) Directory.CreateDirectory(artifactWorkspace);
            
            try {
                // 1. Database Backup
                string? dbBackupFile = null;
                if (!string.IsNullOrEmpty(config.DbConnectionString) && !string.IsNullOrEmpty(config.DbProvider) && config.DbProvider != "None")
                {
                    var provider = _backupProviders.FirstOrDefault(p => p != null && p.ProviderName != null && p.ProviderName.Equals(config.DbProvider, StringComparison.OrdinalIgnoreCase));
                    if (provider == null) throw new Exception($"Database provider '{config.DbProvider}' not found.");

                    await runLogger.LogAsync($"[Database] Backing up '{config.DbProvider}' source...", 10);
                    dbBackupFile = await provider.CreateBackupAsync(config.DbConnectionString, artifactWorkspace);
                    await runLogger.LogAsync($"[Database] Successfully dumped to: {Path.GetFileName(dbBackupFile)}");
                }

                // 2. File Synchronization (Source -> Persistent Sync Workspace)
                SyncResult? syncResult = null;
                if (!string.IsNullOrEmpty(sourceDir))
                {
                    await runLogger.LogAsync($"[FileSync] Scanning source: {sourceDir}...", 30);
                    syncResult = await _syncEngine.SyncAsync(sourceDir, syncWorkspace, job.IgnorePatterns, async (msg) => await runLogger.LogAsync(msg));
                    await runLogger.LogAsync($"[FileSync] Scan results: {syncResult.TotalFilesFound} total files, {syncResult.FilesSynced} changed/new files.");
                }

                // 3. Skip Check
                bool hasChanges = (dbBackupFile != null) || (syncResult != null && syncResult.ChangesDetected);
                if (hasChanges == false && config.SkipIfNoChanges)
                {
                    await runLogger.LogAsync(">>> RESULT: No changes detected. Pipeline skipped.", 100);
                    run.Status = JobStatus.Skipped;
                    run.EndTime = DateTime.UtcNow;
                    await runLogger.UpdateFinalStatusAsync();
                    return JobExecutionResult.Skipped;
                }

                // 4. Artifact Assembly (Sync Workspace -> Artifact Workspace)
                // We MUST copy files to the artifact workspace so they can be zipped/stored
                if (!string.IsNullOrEmpty(sourceDir))
                {
                    await runLogger.LogAsync("[Pipeline] Assembling artifacts for storage...");
                    CopyDirectory(syncWorkspace, artifactWorkspace);
                }

                // 5. Filename Generation & Compression
                string finalArchiveFile = "";
                string extension = config.EnableCompression ? ".7z" : "";
                string fileName = _filenameGenerator.Generate(job.Name, extension);
                
                if (config.EnableCompression)
                {
                    // Create in a specific temp compression folder
                    var compressionDir = Path.Combine(Path.GetTempPath(), "Memlane", "Compression", Guid.NewGuid().ToString().Substring(0, 8));
                    if (!Directory.Exists(compressionDir)) Directory.CreateDirectory(compressionDir);
                    
                    var tempArchivePath = Path.Combine(compressionDir, fileName);
                    
                    await runLogger.LogAsync($"[Compression] Packaging artifacts into 7z archive ({config.CompressionLevel ?? "Normal"} level)...", 70);
                    try {
                        await _compressionProvider.CompressAsync(artifactWorkspace, tempArchivePath, config.CompressionLevel ?? "Normal", async (msg) => await runLogger.LogAsync(msg));
                        finalArchiveFile = tempArchivePath;
                        await runLogger.LogAsync($"[Compression] Successfully created: {fileName} ({new FileInfo(finalArchiveFile).Length / 1024} KB)");
                    } catch (Exception ex) {
                        if (Directory.Exists(compressionDir)) Directory.Delete(compressionDir, true);
                        throw new Exception($"Compression failed: {ex.Message}", ex);
                    }
                }

                // 6. Storage Provider (Move to Final Destination)
                if (!string.IsNullOrEmpty(config.StorageProvider) && !string.IsNullOrEmpty(targetDest))
                {
                    var storage = _storageProviderFactory.GetProvider(config.StorageProvider);
                    var targetPath = Path.Combine(targetDest, fileName);

                    if (config.EnableCompression)
                    {
                        await runLogger.LogAsync($"[Storage] Moving archive to {storage.ProviderName} destination: {targetDest}...", 90);
                        await storage.SaveAsync(finalArchiveFile, targetPath);
                        
                        // Cleanup temp archive and its directory
                        if (File.Exists(finalArchiveFile)) File.Delete(finalArchiveFile);
                        var compressionDir = Path.GetDirectoryName(finalArchiveFile);
                        if (compressionDir != null && Directory.Exists(compressionDir)) Directory.Delete(compressionDir, true);
                    }
                    else
                    {
                        await runLogger.LogAsync($"[Storage] Mirroring workspace to {storage.ProviderName} destination: {targetDest}...", 90);
                        // For non-compressed, we mirror the artifact workspace (which is a copy of sync workspace)
                        // This might be redundant if sync workspace is already persistent, but ensures "Pick -> Move"
                        await storage.SaveAsync(artifactWorkspace, targetPath); 
                    }

                    await runLogger.LogAsync($"[Storage] Successfully secured: {fileName}");

                    // 7. Backup Rotation
                    if (config.RetentionCount > 0 && (config.StorageProvider == "Local" || config.StorageProvider == "Folder"))
                    {
                        await runLogger.LogAsync($"[Rotation] Applying retention policy: Keep last {config.RetentionCount} backups...", 95);
                        await _retentionManager.RotateBackupsAsync(targetDest, config.RetentionCount);
                    }
                }

                // Final Cleanup of temp workspaces
                if (Directory.Exists(artifactWorkspace)) Directory.Delete(artifactWorkspace, true);

                await runLogger.LogAsync(">>> RESULT: Pipeline finished successfully.", 100);
                run.Status = JobStatus.Completed;
                run.EndTime = DateTime.UtcNow;
                await runLogger.UpdateFinalStatusAsync();
                return JobExecutionResult.Completed;

            } catch (Exception ex) {
                await runLogger.LogAsync($">>> [CRITICAL FAILURE] {ex.Message}", 100);
                run.Status = JobStatus.Failed;
                run.ResultMessage = ex.Message;
                run.EndTime = DateTime.UtcNow;
                await runLogger.UpdateFinalStatusAsync();
                return JobExecutionResult.Failed;
            }
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourceDir, targetDir));
            }

            foreach (string newPath in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourceDir, targetDir), true);
            }
        }
    }

    internal class RunLogger
    {
        private readonly JobRun _run;
        private readonly IJobRepository _repository;
        private readonly IHubContext<JobHub> _hubContext;
        private readonly ILogger _systemLogger;
        private readonly StringBuilder _logBuffer = new();

        public RunLogger(JobRun run, IJobRepository repository, IHubContext<JobHub> hubContext, ILogger systemLogger)
        {
            _run = run;
            _repository = repository;
            _hubContext = hubContext;
            _systemLogger = systemLogger;
        }

        public async Task LogAsync(string message, int? progress = null)
        {
            var timestampedMessage = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {message}";
            _logBuffer.AppendLine(timestampedMessage);
            _systemLogger.LogInformation("Job {JobId} Run {RunId}: {Message}", _run.JobId, _run.Id, message);

            _run.Logs = _logBuffer.ToString();
            await _repository.UpdateRunAsync(_run);

            var update = new JobStatusUpdate(_run.JobId, _run.Status.ToString(), message, progress ?? -1);
            if (_hubContext != null) {
                await _hubContext.Clients.Group($"Job_{_run.JobId}").SendAsync("ReceiveStatusUpdate", update);
                await _hubContext.Clients.All.SendAsync("ReceiveGlobalStatusUpdate", update);
            }
        }

        public async Task UpdateFinalStatusAsync()
        {
            _run.Logs = _logBuffer.ToString();
            await _repository.UpdateRunAsync(_run);
            
            var update = new JobStatusUpdate(_run.JobId, _run.Status.ToString(), _run.ResultMessage ?? "Finished", 100);
            if (_hubContext != null) {
                await _hubContext.Clients.Group($"Job_{_run.JobId}").SendAsync("ReceiveStatusUpdate", update);
                await _hubContext.Clients.All.SendAsync("ReceiveGlobalStatusUpdate", update);
            }
        }
    }

    public class BackupJobConfiguration
    {
        [JsonPropertyName("dbProvider")]
        public string? DbProvider { get; set; }
        
        [JsonPropertyName("dbConnectionString")]
        public string? DbConnectionString { get; set; }
        
        [JsonPropertyName("sourceDirectory")]
        public string? SourceDirectory { get; set; }
        
        [JsonPropertyName("storageProvider")]
        public string? StorageProvider { get; set; } 
        
        [JsonPropertyName("targetDestination")]
        public string? TargetDestination { get; set; }
        
        [JsonPropertyName("enableCompression")]
        public bool EnableCompression { get; set; }
        
        [JsonPropertyName("archiveFileName")]
        public string? ArchiveFileName { get; set; }
        
        [JsonPropertyName("skipIfNoChanges")]
        public bool SkipIfNoChanges { get; set; } = true;
        
        [JsonPropertyName("retentionCount")]
        public int RetentionCount { get; set; }

        [JsonPropertyName("compressionLevel")]
        public string? CompressionLevel { get; set; }
    }
}