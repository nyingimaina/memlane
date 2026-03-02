using Memlane.Api.Providers;
using Xunit;

namespace Memlane.Tests
{
    public class ProviderTests
    {
        [Fact]
        public async Task SqlServerBackupProvider_ShouldReturnCorrectFileName()
        {
            var provider = new SqlServerBackupProvider();
            var connectionString = "Server=localhost;Database=TestDB;Integrated Security=SSPI;";
            var targetDir = @"C:\Backups";

            var result = await provider.CreateBackupAsync(connectionString, targetDir);

            Assert.Contains("TestDB", result);
            Assert.EndsWith(".bak", result);
            Assert.StartsWith(targetDir, result);
        }

        [Fact]
        public async Task MariaDbBackupProvider_ShouldReturnCorrectFileName()
        {
            var provider = new MariaDbBackupProvider();
            var connectionString = "Server=localhost;Database=MariaDBTest;User=root;Password=pass;";
            var targetDir = "/tmp/backups";

            var result = await provider.CreateBackupAsync(connectionString, targetDir);

            Assert.Contains("MariaDBTest", result);
            Assert.EndsWith(".sql", result);
            Assert.StartsWith(targetDir, result);
        }
    }
}