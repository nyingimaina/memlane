using System.Formats.Tar;
using System.Text.Json;
using ZstdSharp;

namespace Memlane.Api.Providers
{
    public class ZstdProvider : ICompressionProvider
    {
        private readonly ILogger<ZstdProvider> _logger;

        public ZstdProvider(ILogger<ZstdProvider> logger)
        {
            _logger = logger;
        }

        public string ProviderName => "Zstandard";
        public string DefaultExtension => ".tar.zst";

        public async Task<string> CompressAsync(string sourceDirectory, string targetFilePath, string? optionsJson = null, Action<string>? logger = null)
        {
            if (!targetFilePath.EndsWith(DefaultExtension, StringComparison.OrdinalIgnoreCase))
            {
                targetFilePath += DefaultExtension;
            }

            var options = !string.IsNullOrEmpty(optionsJson) 
                ? JsonSerializer.Deserialize<ZstdOptions>(optionsJson) 
                : new ZstdOptions();

            // Zstd levels range from -5 to 22. Default is 3.
            int level = options?.Level?.ToLower() switch
            {
                "fastest" => 1,
                "fast" => 3,
                "normal" => 7,
                "maximum" => 15,
                "ultra" => 22,
                _ => 3
            };

            logger?.Invoke($"[Zstandard] Creating archive: {Path.GetFileName(targetFilePath)} (Level: {level})");

            try
            {
                await Task.Run(() =>
                {
                    using var outputStream = File.Create(targetFilePath);
                    using var compressionStream = new CompressionStream(outputStream, level);
                    
                    // Tar the directory directly into the compression stream
                    TarFile.CreateFromDirectory(sourceDirectory, compressionStream, includeBaseDirectory: false);
                });

                var fileInfo = new FileInfo(targetFilePath);
                logger?.Invoke($"[Zstandard] Compression complete. Size: {fileInfo.Length / 1024} KB");
                return targetFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ZstdProvider failed.");
                throw;
            }
        }

        private class ZstdOptions
        {
            public string? Level { get; set; }
        }
    }
}
