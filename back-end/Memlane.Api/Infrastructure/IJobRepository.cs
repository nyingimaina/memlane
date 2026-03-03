using Dapper;
using Memlane.Api.Models;

namespace Memlane.Api.Infrastructure
{
    public interface IJobRepository
    {
        Task InitializeAsync();
        Task<IEnumerable<JobMetadata>> GetAllJobsAsync();
        Task<IEnumerable<JobMetadata>> GetPendingJobsAsync();
        Task<IEnumerable<JobMetadata>> GetScheduledJobsToRunAsync();
        Task UpdateJobStatusAsync(int jobId, JobStatus status, string? error = null);
        Task UpdateNextRunTimeAsync(int jobId, DateTime? nextRunAt);
        Task<int> AddJobAsync(JobMetadata job);
        Task<JobMetadata?> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task UpdateAsync(JobMetadata job);
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
                    ConfigurationJson TEXT,
                    CronExpression TEXT,
                    NextRunAt DATETIME
                )");
        }

        public async Task<IEnumerable<JobMetadata>> GetAllJobsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<JobMetadata>("SELECT * FROM Jobs ORDER BY CreatedAt DESC");
        }

        public async Task<IEnumerable<JobMetadata>> GetPendingJobsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<JobMetadata>(
                "SELECT * FROM Jobs WHERE Status = @Status", 
                new { Status = JobStatus.Pending });
        }

        public async Task<IEnumerable<JobMetadata>> GetScheduledJobsToRunAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<JobMetadata>(
                "SELECT * FROM Jobs WHERE NextRunAt <= @Now AND Status != @Status", 
                new { Now = DateTime.UtcNow, Status = JobStatus.InProgress });
        }

        public async Task UpdateJobStatusAsync(int jobId, JobStatus status, string? error = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                "UPDATE Jobs SET Status = @Status, LastRunAt = @LastRunAt, LastError = @LastError WHERE Id = @Id",
                new { Id = jobId, Status = status, LastRunAt = DateTime.UtcNow, LastError = error });
        }

        public async Task UpdateNextRunTimeAsync(int jobId, DateTime? nextRunAt)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                "UPDATE Jobs SET NextRunAt = @NextRunAt WHERE Id = @Id",
                new { Id = jobId, NextRunAt = nextRunAt });
        }

        public async Task<int> AddJobAsync(JobMetadata job)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleAsync<int>(@"
                INSERT INTO Jobs (Name, Type, Status, CreatedAt, ConfigurationJson, CronExpression, NextRunAt) 
                VALUES (@Name, @Type, @Status, @CreatedAt, @ConfigurationJson, @CronExpression, @NextRunAt);
                SELECT last_insert_rowid();",
                job);
        }

        public async Task<JobMetadata?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<JobMetadata>("SELECT * FROM Jobs WHERE Id = @Id", new { Id = id });
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Jobs WHERE Id = @Id", new { Id = id });
        }

        public async Task UpdateAsync(JobMetadata job)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(@"
                UPDATE Jobs 
                SET Name = @Name, 
                    Type = @Type, 
                    ConfigurationJson = @ConfigurationJson,
                    CronExpression = @CronExpression,
                    NextRunAt = @NextRunAt
                WHERE Id = @Id", job);
        }
    }
}