namespace Memlane.Api.Providers
{
    public interface IBackupProvider
    {
        string ProviderName { get; }
        Task<string> CreateBackupAsync(string connectionString, string targetDirectory);
    }

    public interface IStorageProvider
    {
        string ProviderName { get; }
        Task SaveAsync(string sourcePath, string targetPath);
        Task DeleteAsync(string path);
        Task<bool> ExistsAsync(string path);
    }

    public interface ICompressionProvider
    {
        string ProviderName { get; }
        Task<string> CompressAsync(string sourceDirectory, string targetFilePath, string? level = "Normal", Action<string>? logger = null);
    }
}