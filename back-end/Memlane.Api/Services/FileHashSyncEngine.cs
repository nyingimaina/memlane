using System.Security.Cryptography;

namespace Memlane.Api.Services
{
    public interface ISyncEngine
    {
        Task SyncAsync(string sourceDir, string targetDir);
    }

    public class FileHashSyncEngine : ISyncEngine
    {
        private readonly ILogger<FileHashSyncEngine> _logger;

        public FileHashSyncEngine(ILogger<FileHashSyncEngine> logger)
        {
            _logger = logger;
        }

        public async Task SyncAsync(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                _logger.LogWarning("Source directory {SourceDir} does not exist.", sourceDir);
                return;
            }

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            var sourceFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);

            foreach (var sourceFile in sourceFiles)
            {
                var relativePath = Path.GetRelativePath(sourceDir, sourceFile);
                var targetFile = Path.Combine(targetDir, relativePath);

                if (await ShouldSyncAsync(sourceFile, targetFile))
                {
                    _logger.LogInformation("Syncing {RelativePath}...", relativePath);
                    var targetSubDir = Path.GetDirectoryName(targetFile);
                    if (targetSubDir != null && !Directory.Exists(targetSubDir))
                    {
                        Directory.CreateDirectory(targetSubDir);
                    }
                    File.Copy(sourceFile, targetFile, true);
                }
            }
        }

        private async Task<bool> ShouldSyncAsync(string sourceFile, string targetFile)
        {
            if (!File.Exists(targetFile))
            {
                return true;
            }

            var sourceHash = await ComputeHashAsync(sourceFile);
            var targetHash = await ComputeHashAsync(targetFile);

            return sourceHash != targetHash;
        }

        private async Task<string> ComputeHashAsync(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}