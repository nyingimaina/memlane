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
            var syncLogger = new Mock<ILogger<FileHashSyncEngine>>();
            var syncEngine = new FileHashSyncEngine(syncLogger.Object);
            
            var mockBackupProvider = new Mock<IBackupProvider>();
            mockBackupProvider.Setup(p => p.ProviderName).Returns("SQL Server");

            var mockStorageFactory = new Mock<IStorageProviderFactory>();
            var mockStorageProvider = new Mock<IStorageProvider>();
            mockStorageProvider.Setup(s => s.ProviderName).Returns("Folder");
            mockStorageFactory.Setup(f => f.GetProvider(It.IsAny<string>())).Returns(mockStorageProvider.Object);

            var mockRetention = new Mock<IRetentionManager>();
            var mockFilenameGen = new Mock<IFilenameGenerator>();
            mockFilenameGen.Setup(f => f.Generate(It.IsAny<string>(), It.IsAny<string>())).Returns("test_backup.zip");
            
            var mockHubContext = new Mock<IHubContext<JobHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

            var orchestrator = new BackupJobOrchestrator(
                new[] { mockBackupProvider.Object },
                mockStorageFactory.Object,
                syncEngine,
                mockRetention.Object,
                mockFilenameGen.Object,
                mockHubContext.Object,
                new Mock<ILogger<BackupJobOrchestrator>>().Object);

            // Create a file and sync it once
            File.WriteAllText(Path.Combine(_sourceDir, "test.txt"), "Initial Content");
            
            var config = new BackupJobConfiguration
            {
                DbProvider = "None",
                DbConnectionString = "Server=.;Database=Test;",
                SourceDirectory = _sourceDir,
                StorageProvider = "Folder",
                TargetDestination = _targetDir,
                SkipIfNoChanges = true,
                EnableCompression = true
            };

            // USE A UNIQUE JOB ID FOR EACH TEST INSTANCE TO AVOID TEMP DIR COLLISIONS
            var jobId = new Random().Next(10000, 99999);
            var job = new JobMetadata
            {
                Id = jobId,
                Name = "E2E_Test_Job",
                ConfigurationJson = JsonSerializer.Serialize(config)
            };

            // Act - First Run (Should Sync)
            var result1 = await orchestrator.ExecuteJobAsync(job, CancellationToken.None);
            
            // Act - Second Run (Should Skip because files in sync workspace match files in source)
            var result2 = await orchestrator.ExecuteJobAsync(job, CancellationToken.None);

            // Assert
            Assert.Equal(JobExecutionResult.Completed, result1);
            Assert.Equal(JobExecutionResult.Skipped, result2);
            
            // Should have called save once for the first run
            mockStorageProvider.Verify(s => s.SaveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task BackupJob_ShouldNotSkip_WhenFilesHaveChanged()
        {
            // Arrange
            var syncLogger = new Mock<ILogger<FileHashSyncEngine>>();
            var syncEngine = new FileHashSyncEngine(syncLogger.Object);
            
            var mockBackupProvider = new Mock<IBackupProvider>();
            mockBackupProvider.Setup(p => p.ProviderName).Returns("SQL Server");

            var mockStorageFactory = new Mock<IStorageProviderFactory>();
            var mockStorageProvider = new Mock<IStorageProvider>();
            mockStorageProvider.Setup(s => s.ProviderName).Returns("Folder");
            mockStorageFactory.Setup(f => f.GetProvider(It.IsAny<string>())).Returns(mockStorageProvider.Object);

            var mockRetention = new Mock<IRetentionManager>();
            var mockFilenameGen = new Mock<IFilenameGenerator>();
            mockFilenameGen.Setup(f => f.Generate(It.IsAny<string>(), It.IsAny<string>())).Returns("test_backup.zip");
            
            var mockHubContext = new Mock<IHubContext<JobHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

            var orchestrator = new BackupJobOrchestrator(
                new[] { mockBackupProvider.Object },
                mockStorageFactory.Object,
                syncEngine,
                mockRetention.Object,
                mockFilenameGen.Object,
                mockHubContext.Object,
                new Mock<ILogger<BackupJobOrchestrator>>().Object);

            File.WriteAllText(Path.Combine(_sourceDir, "test.txt"), "Initial Content");
            
            var config = new BackupJobConfiguration
            {
                DbProvider = "None",
                SourceDirectory = _sourceDir,
                StorageProvider = "Folder",
                TargetDestination = _targetDir,
                SkipIfNoChanges = true,
                EnableCompression = true
            };

            var jobId = new Random().Next(10000, 99999);
            var job = new JobMetadata
            {
                Id = jobId,
                Name = "E2E_Change_Test",
                ConfigurationJson = JsonSerializer.Serialize(config)
            };

            // Act - First Run
            await orchestrator.ExecuteJobAsync(job, CancellationToken.None);
            
            // Modify file
            File.WriteAllText(Path.Combine(_sourceDir, "test.txt"), "Updated Content");

            // Act - Second Run
            var result2 = await orchestrator.ExecuteJobAsync(job, CancellationToken.None);

            // Assert
            Assert.Equal(JobExecutionResult.Completed, result2);
            mockStorageProvider.Verify(s => s.SaveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task BackupJob_ShouldCallRetentionManager_WhenRotationIsEnabled()
        {
            // Arrange
            var syncLogger = new Mock<ILogger<FileHashSyncEngine>>();
            var syncEngine = new FileHashSyncEngine(syncLogger.Object);
            
            var mockBackupProvider = new Mock<IBackupProvider>();
            mockBackupProvider.Setup(p => p.ProviderName).Returns("SQL Server");
            mockBackupProvider.Setup(p => p.CreateBackupAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("db.bak");

            var mockStorageFactory = new Mock<IStorageProviderFactory>();
            var mockStorageProvider = new Mock<IStorageProvider>();
            mockStorageProvider.Setup(s => s.ProviderName).Returns("Folder");
            mockStorageFactory.Setup(f => f.GetProvider(It.IsAny<string>())).Returns(mockStorageProvider.Object);

            var mockRetention = new Mock<IRetentionManager>();
            var mockFilenameGen = new Mock<IFilenameGenerator>();
            mockFilenameGen.Setup(f => f.Generate(It.IsAny<string>(), It.IsAny<string>())).Returns("rotation_test.zip");
            
            var mockHubContext = new Mock<IHubContext<JobHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

            var orchestrator = new BackupJobOrchestrator(
                new[] { mockBackupProvider.Object },
                mockStorageFactory.Object,
                syncEngine,
                mockRetention.Object,
                mockFilenameGen.Object,
                mockHubContext.Object,
                new Mock<ILogger<BackupJobOrchestrator>>().Object);

            File.WriteAllText(Path.Combine(_sourceDir, "test.txt"), "Initial Content");
            
            var config = new BackupJobConfiguration
            {
                DbProvider = "SQL Server",
                DbConnectionString = "Server=.;Database=Test;",
                SourceDirectory = _sourceDir,
                StorageProvider = "Folder",
                TargetDestination = _targetDir,
                RetentionCount = 5,
                EnableCompression = true
            };

            var job = new JobMetadata
            {
                Id = 999,
                Name = "E2E_Rotation_Job",
                ConfigurationJson = JsonSerializer.Serialize(config)
            };

            // Act
            await orchestrator.ExecuteJobAsync(job, CancellationToken.None); 

            // Assert
            mockRetention.Verify(r => r.RotateBackupsAsync(It.IsAny<string>(), 5), Times.Once);
        }
    }
}