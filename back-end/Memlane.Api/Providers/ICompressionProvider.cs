namespace Memlane.Api.Providers
{
    public interface ICompressionProvider
    {
        string ProviderName { get; }
        string DefaultExtension { get; }
        Task<string> CompressAsync(string sourceDirectory, string targetFilePath, string? optionsJson = null, Action<string>? logger = null);
    }
}
