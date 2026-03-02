using System.Security.Cryptography;
using System.Text;

namespace Memlane.Api.Services
{
    public interface IFilenameGenerator
    {
        string Generate(string source, string extension);
    }

    public class SortableFilenameGenerator : IFilenameGenerator
    {
        public string Generate(string source, string extension)
        {
            var cleanSource = Sanitize(source);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var shortHash = GenerateShortHash(source + timestamp);
            
            var ext = extension.StartsWith(".") ? extension : "." + extension;
            return $"{cleanSource}_Full_{timestamp}_{shortHash}{ext}";
        }

        private string Sanitize(string input)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder();
            foreach (var c in input)
            {
                if (!invalidChars.Contains(c) && c != ' ')
                {
                    sb.Append(c);
                }
                else if (c == ' ')
                {
                    sb.Append('_');
                }
            }
            return sb.ToString();
        }

        private string GenerateShortHash(string input)
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant().Substring(0, 4);
        }
    }
}