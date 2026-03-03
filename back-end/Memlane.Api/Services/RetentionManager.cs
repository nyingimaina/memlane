namespace Memlane.Api.Services
{
    public interface IRetentionManager
    {
        Task RotateBackupsAsync(string targetPath, int retentionCount);
    }

    public class RetentionManager : IRetentionManager
    {
        private readonly ILogger<RetentionManager> _logger;

        public RetentionManager(ILogger<RetentionManager> logger)
        {
            _logger = logger;
        }

        public async Task RotateBackupsAsync(string targetPath, int retentionCount)
        {
            if (retentionCount <= 0) return;

            try {
                _logger.LogInformation("Starting backup rotation at {Path}. Keeping last {Count} backups.", targetPath, retentionCount);

                // If targetPath is a directory, get files. 
                // If it's a file path, get files in that directory matching the pattern.
                string directory;
                string searchPattern = "*";

                if (Directory.Exists(targetPath))
                {
                    directory = targetPath;
                }
                else
                {
                    directory = Path.GetDirectoryName(targetPath) ?? "";
                    var fileName = Path.GetFileName(targetPath);
                    // Try to guess pattern if it looks like it has a timestamp (e.g., contains "_Full_")
                    if (fileName.Contains("_Full_"))
                    {
                        var prefix = fileName.Split("_Full_")[0];
                        searchPattern = $"{prefix}_Full_*";
                    }
                }

                if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory)) return;

                var files = Directory.GetFiles(directory, searchPattern)
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTimeUtc)
                    .ToList();

                if (files.Count > retentionCount)
                {
                    var filesToDelete = files.Skip(retentionCount).ToList();
                    foreach (var file in filesToDelete)
                    {
                        _logger.LogInformation("Deleting old backup: {File}", file.Name);
                        file.Delete();
                    }
                    _logger.LogInformation("Rotation complete. Deleted {DeletedCount} files.", filesToDelete.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during backup rotation at {Path}", targetPath);
            }

            await Task.CompletedTask;
        }
    }
}