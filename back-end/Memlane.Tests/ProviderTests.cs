using Memlane.Api.Providers;
using Memlane.Api.Services;
using Xunit;

namespace Memlane.Tests
{
    public class ProviderTests
    {
        [Fact]
        public async Task SqlServerBackupProvider_ShouldReturnCorrectFileName()
        {
            var generator = new SortableFilenameGenerator();
            var provider = new SqlServerBackupProvider(generator);
            var connectionString = "Server=localhost;Database=TestDB;Integrated Security=SSPI;";
            var targetDir = @"C:\Backups";

            var result = await provider.CreateBackupAsync(connectionString, targetDir);

            Assert.Contains("TestDB_Full_", result);
            Assert.EndsWith(".bak", result);
            Assert.StartsWith(targetDir, result);
        }

        [Fact]
        public async Task MariaDbBackupProvider_ShouldReturnCorrectFileName()
        {
            var generator = new SortableFilenameGenerator();
            var provider = new MariaDbBackupProvider(generator);
            var connectionString = "Server=localhost;Database=MariaDBTest;User=root;Password=pass;";
            var targetDir = "/tmp/backups";

            var result = await provider.CreateBackupAsync(connectionString, targetDir);

            Assert.Contains("MariaDBTest_Full_", result);
            Assert.EndsWith(".sql", result);
            Assert.StartsWith(targetDir, result);
        }
    }
}