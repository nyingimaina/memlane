using Dapper;
using Memlane.Api.Models;

namespace Memlane.Api.Infrastructure
{
    public interface IJobRepository
    {
        Task InitializeAsync();
        Task<IEnumerable<JobMetadata>> GetPendingJobsAsync();
        Task UpdateJobStatusAsync(int jobId, JobStatus status, string? error = null);
        Task<int> AddJobAsync(JobMetadata job);
    }

    public class SqliteJobRepository : IJobRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SqliteJobRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InitializeAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS Jobs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Type TEXT NOT NULL,
                    Status INTEGER NOT NULL,
                    CreatedAt DATETIME NOT NULL,
                    LastRunAt DATETIME,
                    LastError TEXT,
                    ConfigurationJson TEXT
                )");
        }

        public async Task<IEnumerable<JobMetadata>> GetPendingJobsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<JobMetadata>(
                "SELECT * FROM Jobs WHERE Status = @Status", 
                new { Status = JobStatus.Pending });
        }

        public async Task UpdateJobStatusAsync(int jobId, JobStatus status, string? error = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                "UPDATE Jobs SET Status = @Status, LastRunAt = @LastRunAt, LastError = @LastError WHERE Id = @Id",
                new { Id = jobId, Status = status, LastRunAt = DateTime.UtcNow, LastError = error });
        }

        public async Task<int> AddJobAsync(JobMetadata job)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleAsync<int>(@"
                INSERT INTO Jobs (Name, Type, Status, CreatedAt, ConfigurationJson) 
                VALUES (@Name, @Type, @Status, @CreatedAt, @ConfigurationJson);
                SELECT last_insert_rowid();",
                job);
        }
    }
}