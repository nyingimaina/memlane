using Microsoft.Extensions.Logging;
using Moq;
using Memlane.Api.Services;
using Xunit;

namespace Memlane.Tests
{
    public class SyncTests : IDisposable
    {
        private readonly string _testBaseDir;
        private readonly string _sourceDir;
        private readonly string _targetDir;

        public SyncTests()
        {
            _testBaseDir = Path.Combine(Path.GetTempPath(), "MemlaneSyncTests_" + Guid.NewGuid().ToString());
            _sourceDir = Path.Combine(_testBaseDir, "source");
            _targetDir = Path.Combine(_testBaseDir, "target");

            Directory.CreateDirectory(_sourceDir);
            Directory.CreateDirectory(_targetDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testBaseDir))
            {
                Directory.Delete(_testBaseDir, true);
            }
        }

        [Fact]
        public async Task SyncAsync_ShouldCopyNewFiles()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<FileHashSyncEngine>>();
            var engine = new FileHashSyncEngine(loggerMock.Object);
            var fileName = "test.txt";
            var sourceFile = Path.Combine(_sourceDir, fileName);
            File.WriteAllText(sourceFile, "Hello World");

            // Act
            await engine.SyncAsync(_sourceDir, _targetDir);

            // Assert
            var targetFile = Path.Combine(_targetDir, fileName);
            Assert.True(File.Exists(targetFile));
            Assert.Equal("Hello World", File.ReadAllText(targetFile));
        }

        [Fact]
        public async Task SyncAsync_ShouldOnlyCopyModifiedFiles()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<FileHashSyncEngine>>();
            var engine = new FileHashSyncEngine(loggerMock.Object);
            
            var file1 = "file1.txt";
            var file2 = "file2.txt";
            File.WriteAllText(Path.Combine(_sourceDir, file1), "Content 1");
            File.WriteAllText(Path.Combine(_sourceDir, file2), "Content 2");

            // Initial sync
            await engine.SyncAsync(_sourceDir, _targetDir);
            
            // Modify file1
            File.WriteAllText(Path.Combine(_sourceDir, file1), "Modified Content 1");
            var file2WriteTime = File.GetLastWriteTime(Path.Combine(_targetDir, file2));
            
            // Act
            await engine.SyncAsync(_sourceDir, _targetDir);

            // Assert
            Assert.Equal("Modified Content 1", File.ReadAllText(Path.Combine(_targetDir, file1)));
            Assert.Equal(file2WriteTime, File.GetLastWriteTime(Path.Combine(_targetDir, file2)));
        }

        [Fact]
        public async Task SyncAsync_ShouldHandleSubdirectories()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<FileHashSyncEngine>>();
            var engine = new FileHashSyncEngine(loggerMock.Object);
            
            var subDir = Path.Combine(_sourceDir, "sub");
            Directory.CreateDirectory(subDir);
            File.WriteAllText(Path.Combine(subDir, "subfile.txt"), "Sub Content");

            // Act
            await engine.SyncAsync(_sourceDir, _targetDir);

            // Assert
            Assert.True(File.Exists(Path.Combine(_targetDir, "sub", "subfile.txt")));
        }
    }
}