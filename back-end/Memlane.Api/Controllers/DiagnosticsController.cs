using Microsoft.AspNetCore.Mvc;
using SevenZip;

namespace Memlane.Api.Controllers
{
    [ApiController]
    [Route("api/diagnostics")]
    public class DiagnosticsController : ControllerBase
    {
        [HttpGet("sevenzip")]
        public IActionResult GetSevenZipStatus()
        {
            var baseDir = AppContext.BaseDirectory;
            var x64Path = Path.Combine(baseDir, "x64", "7z.dll");
            var x86Path = Path.Combine(baseDir, "x86", "7z.dll");
            var rootPath = Path.Combine(baseDir, "7z.dll");

            return Ok(new
            {
                BaseDirectory = baseDir,
                Is64BitProcess = Environment.Is64BitProcess,
                Paths = new
                {
                    x64 = new { Path = x64Path, Exists = File.Exists(x64Path) },
                    x86 = new { Path = x86Path, Exists = File.Exists(x86Path) },
                    root = new { Path = rootPath, Exists = File.Exists(rootPath) }
                },
                CurrentLibraryPath = GetCurrentLibraryPath()
            });
        }

        private string? GetCurrentLibraryPath()
        {
            try {
                // Accessing private field via reflection if needed, but for now just returning try/catch
                return "Unknown (SevenZipBase doesn't expose current path easily)";
            } catch { return "Error"; }
        }
    }
}
