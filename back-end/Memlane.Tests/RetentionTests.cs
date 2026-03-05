using Microsoft.Extensions.Logging;
using Moq;
using Memlane.Api.Services;
using Xunit;

namespace Memlane.Tests
{
    public class RetentionTests : IDisposable
    {
        private readonly string _testDir;
        private readonly Mock<ILogger<RetentionManager>> _mockLogger;
        private readonly RetentionManager _retentionManager;

        public RetentionTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "MemlaneRetention_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDir);
            _mockLogger = new Mock<ILogger<RetentionManager>>();
            _retentionManager = new RetentionManager(_mockLogger.Object);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }

        [Fact]
        public async Task RotateBackups_ShouldDeleteOldFiles_WhenCountExceeded()
        {
            // Arrange
            // Create 5 files with different creation times
            for (int i = 0; i < 5; i++)
            {
                var filePath = Path.Combine(_testDir, $"backup_{i}.bak");
                File.WriteAllText(filePath, "test");
                // Artificially space out creation times if needed, but OrderByDescending handles it
                File.SetCreationTimeUtc(filePath, DateTime.UtcNow.AddMinutes(-i));
            }

            // Act - Keep only 3
            await _retentionManager.RotateBackupsAsync(_testDir, 3);

            // Assert
            var remainingFiles = Directory.GetFiles(_testDir);
            Assert.Equal(3, remainingFiles.Length);
            
            // Verify that the newest ones are kept (backup_0, backup_1, backup_2)
            // Wait, i=0 is newest (AddMinutes(0)), i=4 is oldest (AddMinutes(-4))
            Assert.Contains(remainingFiles, f => f.EndsWith("backup_0.bak"));
            Assert.Contains(remainingFiles, f => f.EndsWith("backup_1.bak"));
            Assert.Contains(remainingFiles, f => f.EndsWith("backup_2.bak"));
            Assert.DoesNotContain(remainingFiles, f => f.EndsWith("backup_3.bak"));
            Assert.DoesNotContain(remainingFiles, f => f.EndsWith("backup_4.bak"));
        }

        [Fact]
        public async Task RotateBackups_ShouldDoNothing_WhenCountIsZeroOrNegative()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_testDir, "test.bak"), "test");

            // Act
            await _retentionManager.RotateBackupsAsync(_testDir, 0);
            await _retentionManager.RotateBackupsAsync(_testDir, -1);

            // Assert
            Assert.Single(Directory.GetFiles(_testDir));
        }

        [Fact]
        public async Task RotateBackups_ShouldWorkWithFileNamePattern()
        {
            // Arrange
            // Create files matching a pattern
            for (int i = 0; i < 5; i++)
            {
                var filePath = Path.Combine(_testDir, $"MyJob_Full_{DateTime.UtcNow.AddMinutes(-i):yyyyMMddHHmmss}.bak");
                File.WriteAllText(filePath, "test");
            }
            // Create a file NOT matching the pattern
            File.WriteAllText(Path.Combine(_testDir, "OtherJob_Full_20230101.bak"), "other");

            // Act - Target one of the MyJob files
            var targetSample = Path.Combine(_testDir, "MyJob_Full_20230101.bak"); 
            await _retentionManager.RotateBackupsAsync(targetSample, 2);

            // Assert
            var allFiles = Directory.GetFiles(_testDir);
            var myJobFiles = allFiles.Where(f => f.Contains("MyJob_Full_")).ToList();
            
            Assert.Equal(2, myJobFiles.Count);
            Assert.Contains(allFiles, f => f.Contains("OtherJob_Full_")); // Should still be there
        }
    }
}