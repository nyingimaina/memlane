using System.Security.Cryptography;

namespace Memlane.Api.Services
{
    public record SyncResult(bool ChangesDetected, int TotalFilesFound, int FilesSynced);

    public interface ISyncEngine
    {
        Task<SyncResult> SyncAsync(string sourceDir, string targetDir, string? ignorePatterns = null, Action<string>? logger = null);
    }

    public class FileHashSyncEngine : ISyncEngine
    {
        private readonly ILogger<FileHashSyncEngine> _sysLogger;

        public FileHashSyncEngine(ILogger<FileHashSyncEngine> logger)
        {
            _sysLogger = logger;
        }

        public async Task<SyncResult> SyncAsync(string sourceDir, string targetDir, string? ignorePatterns = null, Action<string>? logger = null)
        {
            if (!Directory.Exists(sourceDir))
            {
                var msg = $"Source directory {sourceDir} does not exist.";
                _sysLogger.LogWarning(msg);
                logger?.Invoke($"[WARN] {msg}");
                return new SyncResult(false, 0, 0);
            }

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            var allSourceFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
            
            // Filter files based on ignore patterns
            var ignoredCount = 0;
            var patterns = (ignorePatterns ?? "")
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p) && !p.StartsWith("#"))
                .ToList();

            var sourceFiles = allSourceFiles.Where(f => {
                var relativePath = Path.GetRelativePath(sourceDir, f);
                if (IsIgnored(relativePath, patterns)) {
                    ignoredCount++;
                    return false;
                }
                return true;
            }).ToArray();

            if (ignoredCount > 0) {
                logger?.Invoke($"[FileSync] Ignored {ignoredCount} files based on patterns.");
            }

            var targetFiles = Directory.GetFiles(targetDir, "*", SearchOption.AllDirectories);
            
            int filesSynced = 0;
            bool changesDetected = false;

            logger?.Invoke($"Scanning {sourceFiles.Length} source files...");

            // 1. Check for New or Changed Files
            foreach (var sourceFile in sourceFiles)
            {
                var relativePath = Path.GetRelativePath(sourceDir, sourceFile);
                var targetFile = Path.Combine(targetDir, relativePath);

                if (await ShouldSyncAsync(sourceFile, targetFile, logger))
                {
                    changesDetected = true;
                    var targetSubDir = Path.GetDirectoryName(targetFile);
                    if (targetSubDir != null && !Directory.Exists(targetSubDir))
                    {
                        Directory.CreateDirectory(targetSubDir);
                    }
                    File.Copy(sourceFile, targetFile, true);
                    filesSynced++;
                }
            }

            // 2. Check for Deleted Files (Clean up the sync workspace)
            foreach (var targetFile in targetFiles)
            {
                var relativePath = Path.GetRelativePath(targetDir, targetFile);
                var sourceFile = Path.Combine(sourceDir, relativePath);

                // If file no longer exists in source OR it is now ignored, remove from sync workspace
                if (!File.Exists(sourceFile) || IsIgnored(relativePath, patterns))
                {
                    logger?.Invoke($"[FileSync] Removing {relativePath} from workspace (deleted or ignored).");
                    File.Delete(targetFile);
                    changesDetected = true;
                }
            }

            return new SyncResult(changesDetected, sourceFiles.Length, filesSynced);
        }

        private bool IsIgnored(string relativePath, List<string> patterns)
        {
            if (patterns.Count == 0) return false;

            // Simple .gitignore-like matching
            // Normalize path to use forward slashes for matching
            var normalizedPath = relativePath.Replace("\\", "/");

            foreach (var pattern in patterns)
            {
                var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
                    .Replace("\\*", ".*")
                    .Replace("\\?", ".")
                    .Replace("/", "[\\\\/]") + "($|[\\\\/].*)";
                
                // If pattern contains a slash, it's relative to root, otherwise it matches anywhere
                if (!pattern.Contains("/")) {
                    regexPattern = "(^|[\\\\/])" + System.Text.RegularExpressions.Regex.Escape(pattern)
                        .Replace("\\*", ".*")
                        .Replace("\\?", ".") + "($|[\\\\/].*)";
                }

                if (System.Text.RegularExpressions.Regex.IsMatch(normalizedPath, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> ShouldSyncAsync(string sourceFile, string targetFile, Action<string>? logger)
        {
            if (!File.Exists(targetFile))
            {
                logger?.Invoke($"[FileSync] New file detected: {Path.GetFileName(sourceFile)}");
                return true;
            }

            var sourceHash = await ComputeHashAsync(sourceFile);
            var targetHash = await ComputeHashAsync(targetFile);

            if (sourceHash != targetHash) {
                logger?.Invoke($"[FileSync] Hash changed for {Path.GetFileName(sourceFile)} (S:{sourceHash.Substring(0,8)}... vs T:{targetHash.Substring(0,8)}...)");
                return true;
            }

            return false;
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