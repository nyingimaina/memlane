using System.Data;
using Dapper;
using Polly;
using Polly.Retry;
using Memlane.Api.Infrastructure;
using Memlane.Api.Models;
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
                    CreatedAt = DateTime.UtcNow
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
    }
}