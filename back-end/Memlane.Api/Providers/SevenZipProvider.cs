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
            
            try {
                var baseDir = AppContext.BaseDirectory;
                var x64Path = Path.Combine(baseDir, "x64", "7z.dll");
                var x86Path = Path.Combine(baseDir, "x86", "7z.dll");
                var rootPath = Path.Combine(baseDir, "7z.dll");

                string? selectedPath = null;
                if (Environment.Is64BitProcess && File.Exists(x64Path)) selectedPath = x64Path;
                else if (!Environment.Is64BitProcess && File.Exists(x86Path)) selectedPath = x86Path;
                else if (File.Exists(rootPath)) selectedPath = rootPath;

                if (selectedPath != null)
                {
                    // Explicitly load the library to verify it's valid
                    var handle = NativeLibrary.Load(selectedPath);
                    if (handle != IntPtr.Zero)
                    {
                        SevenZipBase.SetLibraryPath(selectedPath);
                        _logger.LogInformation("7-Zip library successfully loaded from: {Path}", selectedPath);
                    }
                    else
                    {
                        _logger.LogError("NativeLibrary.Load failed for: {Path}", selectedPath);
                    }
                }
                else
                {
                    _logger.LogWarning("7z.dll not found in standard locations. BaseDir: {BaseDir}", baseDir);
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "Error initializing 7-Zip library path. If running in IIS/Express, ensure DLLs are in the bin folder.");
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

            logger?.Invoke($"[7-Zip] Using native library to package artifacts (Level: {level})...");

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
                logger?.Invoke($"[7-Zip-ERR] {ex.Message}");
                throw;
            }
        }

        private class SevenZipOptions
        {
            public string? Level { get; set; }
        }
    }
}
