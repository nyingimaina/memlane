using System.IO.Compression;
using System.Text.Json;

namespace Memlane.Api.Providers
{
    public class ZipProvider : ICompressionProvider
    {
        private readonly ILogger<ZipProvider> _logger;

        public ZipProvider(ILogger<ZipProvider> logger)
        {
            _logger = logger;
        }

        public string ProviderName => "Zip";
        public string DefaultExtension => ".zip";

        public async Task<string> CompressAsync(string sourceDirectory, string targetFilePath, string? optionsJson = null, Action<string>? logger = null)
        {
            if (!targetFilePath.EndsWith(DefaultExtension, StringComparison.OrdinalIgnoreCase))
            {
                targetFilePath += DefaultExtension;
            }

            var options = !string.IsNullOrEmpty(optionsJson) 
                ? JsonSerializer.Deserialize<ZipOptions>(optionsJson) 
                : new ZipOptions();

            var level = options?.Level?.ToLower() switch
            {
                "fastest" => CompressionLevel.Fastest,
                "optimal" => CompressionLevel.Optimal,
                "none" => CompressionLevel.NoCompression,
                _ => CompressionLevel.Optimal
            };

            logger?.Invoke($"[Zip] Starting compression (Level: {level})...");

            try
            {
                await Task.Run(() => ZipFile.CreateFromDirectory(sourceDirectory, targetFilePath, level, false));
                var fileInfo = new FileInfo(targetFilePath);
                logger?.Invoke($"[Zip] Compression complete. Size: {fileInfo.Length / 1024} KB");
                return targetFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ZipProvider failed.");
                throw;
            }
        }

        private class ZipOptions
        {
            public string? Level { get; set; }
        }
    }
}
