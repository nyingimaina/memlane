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
        public string ProviderName => "Local";

        public async Task SaveAsync(string sourcePath, string targetPath)
        {
            // targetPath is expected to be the FULL file path including name
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
            if (File.Exists(path)) File.Delete(path);
            else if (Directory.Exists(path)) Directory.Delete(path, true);
            await Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(File.Exists(path) || Directory.Exists(path));
        }
    }

    public class FolderStorageProvider : IStorageProvider
    {
        public string ProviderName => "Folder";

        public async Task SaveAsync(string sourcePath, string targetPath)
        {
            // Standardize behavior: targetPath is the final destination file path
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
            if (File.Exists(path)) File.Delete(path);
            await Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(File.Exists(path));
        }
    }

    public class S3StorageProvider : IStorageProvider
    {
        public string ProviderName => "S3";

        public async Task SaveAsync(string sourcePath, string targetPath)
        {
            Console.WriteLine($"[S3 STUB] Uploading {sourcePath} to s3://{targetPath}");
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(string path)
        {
            Console.WriteLine($"[S3 STUB] Deleting s3://{path}");
            await Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(false);
        }
    }

    public interface IStorageProviderFactory
    {
        IStorageProvider GetProvider(string name);
    }

    public class StorageProviderFactory : IStorageProviderFactory
    {
        private readonly IEnumerable<IStorageProvider> _providers;

        public StorageProviderFactory(IEnumerable<IStorageProvider> providers)
        {
            _providers = providers;
        }

        public IStorageProvider GetProvider(string name)
        {
            var provider = _providers.FirstOrDefault(p => p.ProviderName.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (provider == null) throw new Exception($"Storage provider '{name}' not found.");
            return provider;
        }
    }
}