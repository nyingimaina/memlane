using System.IO.Compression;

namespace Memlane.Api.Providers
{
    public static class CompressionUtility
    {
        public static async Task CompressAsync(string sourcePath, string targetZipPath)
        {
            if (File.Exists(targetZipPath))
            {
                File.Delete(targetZipPath);
            }

            if (Directory.Exists(sourcePath))
            {
                ZipFile.CreateFromDirectory(sourcePath, targetZipPath);
            }
            else if (File.Exists(sourcePath))
            {
                using var zip = ZipFile.Open(targetZipPath, ZipArchiveMode.Create);
                zip.CreateEntryFromFile(sourcePath, Path.GetFileName(sourcePath));
            }
            await Task.CompletedTask;
        }

        public static async Task DecompressAsync(string zipPath, string targetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            ZipFile.ExtractToDirectory(zipPath, targetDirectory);
            await Task.CompletedTask;
        }
    }

    public class LocalStorageProvider : IStorageProvider
    {
        public string ProviderName => "Local Storage";

        public async Task SaveAsync(string sourcePath, string targetPath)
        {
            var targetDir = Path.GetDirectoryName(targetPath);
            if (targetDir != null && !Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            File.Copy(sourcePath, targetPath, true);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            await Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(File.Exists(path) || Directory.Exists(path));
        }
    }
}