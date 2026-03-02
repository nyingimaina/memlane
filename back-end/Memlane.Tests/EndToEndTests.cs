using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Memlane.Api.Hubs;
using Memlane.Api.Models;
using Memlane.Api.Providers;
using Memlane.Api.Services;
using Xunit;

namespace Memlane.Tests
{
    public class EndToEndTests : IDisposable
    {
        private readonly string _testBaseDir;
        private readonly string _sourceDir;
        private readonly string _targetDir;

        public EndToEndTests()
        {
            _testBaseDir = Path.Combine(Path.GetTempPath(), "MemlaneE2E_" + Guid.NewGuid().ToString());
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
        public async Task BackupJob_ShouldSkip_WhenNoChangesAreDetected_InIntegratedScenario()
        {
            // Arrange
            var logger = new Mock<ILogger<FileHashSyncEngine>>();
            var syncEngine = new FileHashSyncEngine(logger.Object);
            var filenameGen = new SortableFilenameGenerator();
            
            var mockBackupProvider = new Mock<IBackupProvider>();
            mockBackupProvider.Setup(p => p.ProviderName).Returns("SQL Server");
            
            var mockHubContext = new Mock<IHubContext<JobHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

            var orchestrator = new BackupJobOrchestrator(
                new[] { mockBackupProvider.Object },
                new[] { new LocalStorageProvider() },
                syncEngine,
                mockHubContext.Object,
                new Mock<ILogger<BackupJobOrchestrator>>().Object);

            // Create a file and sync it once
            File.WriteAllText(Path.Combine(_sourceDir, "test.txt"), "Initial Content");
            
            var config = new BackupJobConfiguration
            {
                DbProvider = "SQL Server",
                DbConnectionString = "Server=.;Database=Test;",
                SourceDirectory = _sourceDir,
                TargetDirectory = _targetDir,
                SkipIfNoChanges = true
            };

            var job = new JobMetadata
            {
                Id = 1,
                Name = "E2E_Test_Job",
                ConfigurationJson = JsonSerializer.Serialize(config)
            };

            // Act - First Run (Should Sync)
            var result1 = await orchestrator.ExecuteJobAsync(job, CancellationToken.None);
            
            // Act - Second Run (Should Skip)
            var result2 = await orchestrator.ExecuteJobAsync(job, CancellationToken.None);

            // Assert
            Assert.Equal(JobExecutionResult.Completed, result1);
            Assert.Equal(JobExecutionResult.Skipped, result2);
            
            mockBackupProvider.Verify(p => p.CreateBackupAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task BackupJob_ShouldNotSkip_WhenForceBackupIsConfigured()
        {
            // Arrange
            var logger = new Mock<ILogger<FileHashSyncEngine>>();
            var syncEngine = new FileHashSyncEngine(logger.Object);
            
            var mockBackupProvider = new Mock<IBackupProvider>();
            mockBackupProvider.Setup(p => p.ProviderName).Returns("SQL Server");
            mockBackupProvider.Setup(p => p.CreateBackupAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("db.bak");
            
            var mockHubContext = new Mock<IHubContext<JobHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

            var orchestrator = new BackupJobOrchestrator(
                new[] { mockBackupProvider.Object },
                new[] { new LocalStorageProvider() },
                syncEngine,
                mockHubContext.Object,
                new Mock<ILogger<BackupJobOrchestrator>>().Object);

            File.WriteAllText(Path.Combine(_sourceDir, "test.txt"), "Initial Content");
            
            var config = new BackupJobConfiguration
            {
                DbProvider = "SQL Server",
                DbConnectionString = "Server=.;Database=Test;",
                SourceDirectory = _sourceDir,
                TargetDirectory = _targetDir,
                SkipIfNoChanges = false // FORCE BACKUP
            };

            var job = new JobMetadata
            {
                Id = 1,
                Name = "E2E_Force_Job",
                ConfigurationJson = JsonSerializer.Serialize(config)
            };

            // Act
            await orchestrator.ExecuteJobAsync(job, CancellationToken.None); // Sync first
            var result = await orchestrator.ExecuteJobAsync(job, CancellationToken.None); // Run again

            // Assert
            Assert.Equal(JobExecutionResult.Completed, result);
            mockBackupProvider.Verify(p => p.CreateBackupAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }
}