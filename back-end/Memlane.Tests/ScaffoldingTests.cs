using System.Data;
using System.Text.Json;
using Dapper;
using Polly;
using Polly.Retry;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Memlane.Api.Hubs;
using Memlane.Api.Infrastructure;
using Memlane.Api.Models;
using Memlane.Api.Providers;
using Memlane.Api.Services;
using Xunit;

namespace Memlane.Tests
{
    public class ScaffoldingTests
    {
        [Fact]
        public void Project_ShouldBuild_AndRunTests()
        {
            Assert.True(true);
        }

        [Fact]
        public void DbConnectionFactory_ShouldCreateConnection()
        {
            var factory = new SqliteConnectionFactory("Data Source=:memory:");
            using var connection = factory.CreateConnection();
            Assert.NotNull(connection);
            Assert.Equal("Microsoft.Data.Sqlite.SqliteConnection", connection.GetType().FullName);
        }

        [Fact]
        public void SQLite_Dapper_ShouldWork()
        {
            var factory = new SqliteConnectionFactory("Data Source=:memory:");
            using var connection = factory.CreateConnection();
            connection.Open();
            connection.Execute("CREATE TABLE Test (Id INTEGER PRIMARY KEY, Name TEXT)");
            connection.Execute("INSERT INTO Test (Name) VALUES (@Name)", new { Name = "TestItem" });
            var name = connection.QuerySingle<string>("SELECT Name FROM Test WHERE Id = 1");
            Assert.Equal("TestItem", name);
        }

        [Fact]
        public void Polly_ShouldRetry()
        {
            var retryCount = 0;
            var retryPolicy = Policy
                .Handle<Exception>()
                .Retry(3, (exception, retry) => retryCount = retry);

            try
            {
                retryPolicy.Execute(() => throw new Exception("Test exception"));
            }
            catch
            {
                // Expected
            }

            Assert.Equal(3, retryCount);
        }

        [Fact]
        public async Task SqliteJobRepository_ShouldStoreAndRetrieveJobs()
        {
            // Use a physical file for testing to avoid in-memory connection closing issues between Dapper calls
            var dbPath = "test_jobs.db";
            if (File.Exists(dbPath)) File.Delete(dbPath);

            try
            {
                var factory = new SqliteConnectionFactory($"Data Source={dbPath}");
                var repository = new SqliteJobRepository(factory);
                await repository.InitializeAsync();

                var jobId = await repository.AddJobAsync(new JobMetadata
                {
                    Name = "TestJob",
                    Type = "Backup",
                    Status = JobStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    ConfigurationJson = "{}"
                });

                var pendingJobs = await repository.GetPendingJobsAsync();
                Assert.Single(pendingJobs);
                Assert.Equal(jobId, pendingJobs.First().Id);

                await repository.UpdateJobStatusAsync(jobId, JobStatus.Completed);
                pendingJobs = await repository.GetPendingJobsAsync();
                Assert.Empty(pendingJobs);

                // Force clear connection pool to allow file deletion
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            }
            finally
            {
                if (File.Exists(dbPath)) File.Delete(dbPath);
            }
        }

        [Fact]
        public async Task BackupProvider_Contract_ShouldBeMockable()
        {
            var mockProvider = new Mock<IBackupProvider>();
            mockProvider.Setup(p => p.ProviderName).Returns("MockProvider");
            mockProvider.Setup(p => p.CreateBackupAsync(It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync("backup_file.bak");

            var provider = mockProvider.Object;
            Assert.Equal("MockProvider", provider.ProviderName);
            var result = await provider.CreateBackupAsync("connection_string", "target_dir");
            Assert.Equal("backup_file.bak", result);
        }

        [Fact]
        public async Task StorageProvider_Contract_ShouldBeMockable()
        {
            var mockProvider = new Mock<IStorageProvider>();
            mockProvider.Setup(p => p.ProviderName).Returns("MockStorage");
            mockProvider.Setup(p => p.ExistsAsync("test_path")).ReturnsAsync(true);

            var provider = mockProvider.Object;
            Assert.Equal("MockStorage", provider.ProviderName);
            Assert.True(await provider.ExistsAsync("test_path"));
        }

        [Fact]
        public async Task BackupJobOrchestrator_ShouldExecuteAllSteps_AndSendUpdates()
        {
            // Arrange
            var mockBackupProvider = new Mock<IBackupProvider>();
            mockBackupProvider.Setup(p => p.ProviderName).Returns("SQL Server");
            mockBackupProvider.Setup(p => p.CreateBackupAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("db.bak");

            var mockStorageProvider = new Mock<IStorageProvider>();
            var mockSyncEngine = new Mock<ISyncEngine>();
            var mockLogger = new Mock<ILogger<BackupJobOrchestrator>>();

            // SignalR Mocking
            var mockHubContext = new Mock<IHubContext<JobHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

            var orchestrator = new BackupJobOrchestrator(
                new[] { mockBackupProvider.Object },
                new[] { mockStorageProvider.Object },
                mockSyncEngine.Object,
                mockHubContext.Object,
                mockLogger.Object);

            var config = new BackupJobConfiguration
            {
                DbProvider = "SQL Server",
                DbConnectionString = "Server=.;Database=Test;",
                SourceDirectory = Path.Combine(Path.GetTempPath(), "source"),
                TargetDirectory = Path.Combine(Path.GetTempPath(), "target"),
                EnableCompression = false
            };

            var job = new JobMetadata
            {
                Id = 1,
                Name = "FullBackup",
                ConfigurationJson = JsonSerializer.Serialize(config)
            };

            // Act
            await orchestrator.ExecuteJobAsync(job, CancellationToken.None);

            // Assert
            mockBackupProvider.Verify(p => p.CreateBackupAsync(config.DbConnectionString, It.IsAny<string>()), Times.Once);
            mockSyncEngine.Verify(e => e.SyncAsync(config.SourceDirectory, config.TargetDirectory), Times.Once);
            
            // Verify that progress updates were sent
            mockClientProxy.Verify(c => c.SendCoreAsync("ReceiveStatusUpdate", It.Is<object[]>(o => ((JobStatusUpdate)o[0]).Status == "Completed"), default), Times.Once);
        }
    }
}