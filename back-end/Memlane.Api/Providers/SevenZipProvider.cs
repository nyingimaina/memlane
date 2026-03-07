using System.Runtime.InteropServices;
using System.Text.Json;
using SevenZip;

namespace Memlane.Api.Providers
{
    public class SevenZipProvider : ICompressionProvider
    {
        private readonly ILogger<SevenZipProvider> _logger;

        public SevenZipProvider(ILogger<SevenZipProvider> logger)
        {
            _logger = logger;
            
            // Set library path for SevenZipSharp.Interop
            var libPath = Path.Combine(AppContext.BaseDirectory, Environment.Is64BitProcess ? "x64" : "x86", "7z.dll");
            if (File.Exists(libPath))
            {
                SevenZipBase.SetLibraryPath(libPath);
            }
            else {
                libPath = Path.Combine(AppContext.BaseDirectory, "7z.dll");
                if (File.Exists(libPath)) SevenZipBase.SetLibraryPath(libPath);
            }
        }

        public string ProviderName => "7-Zip";
        public string DefaultExtension => ".7z";

        public async Task<string> CompressAsync(string sourceDirectory, string targetFilePath, string? optionsJson = null, Action<string>? logger = null)
        {
            if (!targetFilePath.EndsWith(DefaultExtension, StringComparison.OrdinalIgnoreCase))
            {
                targetFilePath += DefaultExtension;
            }

            var options = !string.IsNullOrEmpty(optionsJson) 
                ? JsonSerializer.Deserialize<SevenZipOptions>(optionsJson) 
                : new SevenZipOptions();

            // SevenZipSharp CompressionLevel: None, Fast, Normal, Ultra
            var level = options?.Level?.ToLower() switch
            {
                "fastest" => CompressionLevel.Fast,
                "fast" => CompressionLevel.Fast,
                "normal" => CompressionLevel.Normal,
                "maximum" => CompressionLevel.Ultra,
                "ultra" => CompressionLevel.Ultra,
                _ => CompressionLevel.Normal
            };

            logger?.Invoke($"[7-Zip] Starting compression (Level: {level})...");

            var compressor = new SevenZipCompressor
            {
                CompressionLevel = level,
                ArchiveFormat = OutArchiveFormat.SevenZip
            };

            try
            {
                await Task.Run(() => compressor.CompressDirectory(sourceDirectory, targetFilePath));
                var fileInfo = new FileInfo(targetFilePath);
                logger?.Invoke($"[7-Zip] Compression complete. Size: {fileInfo.Length / 1024} KB");
                return targetFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SevenZipProvider failed.");
                throw;
            }
        }

        private class SevenZipOptions
        {
            public string? Level { get; set; }
        }
    }
}
