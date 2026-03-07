using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Memlane.Api.Providers
{
    public class SevenZipCommandLineProvider : ICompressionProvider
    {
        private readonly string _sevenZipPath;
        private readonly ILogger<SevenZipCommandLineProvider> _logger;

        public SevenZipCommandLineProvider(ILogger<SevenZipCommandLineProvider> logger, IConfiguration config)
        {
            _logger = logger;
            
            // 7-Zip.CommandLine NuGet puts 7za.exe in the output directory
            // On Windows it is 7za.exe, on Linux it is usually 7za
            var exeName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "7za.exe" : "7za";
            _sevenZipPath = config["Compression:SevenZipPath"] ?? Path.Combine(AppContext.BaseDirectory, exeName);
        }

        public string ProviderName => "7-Zip (CLI)";

        public async Task<string> CompressAsync(string sourceDirectory, string targetFilePath, string? level = "Normal", Action<string>? logger = null)
        {
            if (!targetFilePath.EndsWith(".7z", StringComparison.OrdinalIgnoreCase))
            {
                targetFilePath += ".7z";
            }

            // Ensure parent directory exists
            var dir = Path.GetDirectoryName(targetFilePath);
            if (dir != null && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            // Map levels to 7z switches
            string mx = level?.ToLower() switch
            {
                "fastest" => "1",
                "fast" => "3",
                "normal" => "5",
                "maximum" => "7",
                "ultra" => "9",
                _ => "5"
            };

            // Arguments: a (add), -mx (level), -y (assume yes)
            // Using -t7z explicitly
            var args = $"a -t7z \"{targetFilePath}\" \"{sourceDirectory}\\*\" -mx={mx} -y";
            
            logger?.Invoke($"[7z-CLI] Executing: {_sevenZipPath} {args}");

            var startInfo = new ProcessStartInfo
            {
                FileName = _sevenZipPath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            
            process.OutputDataReceived += (s, e) => {
                if (!string.IsNullOrEmpty(e.Data)) logger?.Invoke($"[7z] {e.Data}");
            };
            process.ErrorDataReceived += (s, e) => {
                if (!string.IsNullOrEmpty(e.Data)) logger?.Invoke($"[7z-ERR] {e.Data}");
            };

            try 
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"7-Zip CLI failed with exit code {process.ExitCode}");
                }

                if (!File.Exists(targetFilePath))
                {
                    throw new FileNotFoundException("7-Zip CLI reported success but target file was not found.");
                }

                var fileInfo = new FileInfo(targetFilePath);
                logger?.Invoke($"[7z-CLI] Compression complete. Size: {fileInfo.Length / 1024} KB");
                return targetFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "7-Zip CLI execution failed.");
                throw;
            }
        }
    }
}
