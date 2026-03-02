using System.Text.Json;
using Memlane.Api.Models;
using Memlane.Api.Providers;

namespace Memlane.Api.Services
{
    public interface IJobOrchestrator
    {
        Task ExecuteJobAsync(JobMetadata job, CancellationToken stoppingToken);
    }

    public class BackupJobOrchestrator : IJobOrchestrator
    {
        private readonly IEnumerable<IBackupProvider> _backupProviders;
        private readonly IEnumerable<IStorageProvider> _storageProviders;
        private readonly ISyncEngine _syncEngine;
        private readonly ILogger<BackupJobOrchestrator> _logger;

        public BackupJobOrchestrator(
            IEnumerable<IBackupProvider> backupProviders,
            IEnumerable<IStorageProvider> storageProviders,
            ISyncEngine syncEngine,
            ILogger<BackupJobOrchestrator> logger)
        {
            _backupProviders = backupProviders;
            _storageProviders = storageProviders;
            _syncEngine = syncEngine;
            _logger = logger;
        }

        public async Task ExecuteJobAsync(JobMetadata job, CancellationToken stoppingToken)
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

            // 1. Database Backup (if applicable)
            string? backupFilePath = null;
            if (!string.IsNullOrEmpty(config.DbConnectionString) && !string.IsNullOrEmpty(config.DbProvider))
            {
                var provider = _backupProviders.FirstOrDefault(p => p.ProviderName.Equals(config.DbProvider, StringComparison.OrdinalIgnoreCase));
                if (provider == null)
                {
                    throw new Exception($"Backup provider '{config.DbProvider}' not found.");
                }

                _logger.LogInformation("Creating database backup using {Provider}...", provider.ProviderName);
                var tempDir = Path.Combine(Path.GetTempPath(), "Memlane_Temp_" + job.Id);
                if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
                
                backupFilePath = await provider.CreateBackupAsync(config.DbConnectionString, tempDir);
                _logger.LogInformation("Database backup created at {Path}", backupFilePath);
            }

            // 2. File Synchronization (if applicable)
            if (!string.IsNullOrEmpty(config.SourceDirectory) && !string.IsNullOrEmpty(config.TargetDirectory))
            {
                _logger.LogInformation("Starting file synchronization from {Source} to {Target}...", config.SourceDirectory, config.TargetDirectory);
                await _syncEngine.SyncAsync(config.SourceDirectory, config.TargetDirectory);
                _logger.LogInformation("File synchronization complete.");
            }

            // 3. Compression (if enabled)
            if (config.EnableCompression && !string.IsNullOrEmpty(config.TargetDirectory) && !string.IsNullOrEmpty(config.ArchiveFileName))
            {
                var archivePath = Path.Combine(config.TargetDirectory, config.ArchiveFileName);
                _logger.LogInformation("Compressing backup to {ArchivePath}...", archivePath);
                await CompressionUtility.CompressAsync(config.TargetDirectory, archivePath);
                _logger.LogInformation("Compression complete.");
            }

            _logger.LogInformation("Backup job {JobId} finished successfully.", job.Id);
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
    }
}