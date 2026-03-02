using Memlane.Api.Providers;
using Xunit;

namespace Memlane.Tests
{
    public class StorageTests : IDisposable
    {
        private readonly string _testBaseDir;

        public StorageTests()
        {
            _testBaseDir = Path.Combine(Path.GetTempPath(), "MemlaneStorageTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testBaseDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testBaseDir))
            {
                Directory.Delete(_testBaseDir, true);
            }
        }

        [Fact]
        public async Task CompressionUtility_ShouldCompressAndDecompress()
        {
            // Arrange
            var sourceDir = Path.Combine(_testBaseDir, "source");
            Directory.CreateDirectory(sourceDir);
            File.WriteAllText(Path.Combine(sourceDir, "test.txt"), "Compressed Content");
            
            var zipPath = Path.Combine(_testBaseDir, "test.zip");
            var extractDir = Path.Combine(_testBaseDir, "extract");

            // Act
            await CompressionUtility.CompressAsync(sourceDir, zipPath);
            Assert.True(File.Exists(zipPath));

            await CompressionUtility.DecompressAsync(zipPath, extractDir);

            // Assert
            Assert.True(File.Exists(Path.Combine(extractDir, "test.txt")));
            Assert.Equal("Compressed Content", File.ReadAllText(Path.Combine(extractDir, "test.txt")));
        }

        [Fact]
        public async Task LocalStorageProvider_ShouldSaveAndDelete()
        {
            // Arrange
            var provider = new LocalStorageProvider();
            var sourceFile = Path.Combine(_testBaseDir, "source.txt");
            var targetFile = Path.Combine(_testBaseDir, "target.txt");
            File.WriteAllText(sourceFile, "Storage Content");

            // Act & Assert
            await provider.SaveAsync(sourceFile, targetFile);
            Assert.True(await provider.ExistsAsync(targetFile));
            Assert.Equal("Storage Content", File.ReadAllText(targetFile));

            await provider.DeleteAsync(targetFile);
            Assert.False(await provider.ExistsAsync(targetFile));
        }
    }
}