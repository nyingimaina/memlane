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

        // Job Run History
        Task<int> AddRunAsync(JobRun run);
        Task UpdateRunAsync(JobRun run);
        Task<IEnumerable<JobRun>> GetRunsByJobIdAsync(int jobId, int limit = 50);
        Task<JobRun?> GetRunByIdAsync(int runId);
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

            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS JobRuns (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    JobId INTEGER NOT NULL,
                    StartTime DATETIME NOT NULL,
                    EndTime DATETIME,
                    Status INTEGER NOT NULL,
                    Logs TEXT,
                    ResultMessage TEXT,
                    FOREIGN KEY(JobId) REFERENCES Jobs(Id) ON DELETE CASCADE
                )");

            var columnNames = await connection.QueryAsync<string>("SELECT name FROM pragma_table_info('Jobs')");
            
            if (!columnNames.Contains("CronExpression"))
            {
                await connection.ExecuteAsync("ALTER TABLE Jobs ADD COLUMN CronExpression TEXT");
            }
            if (!columnNames.Contains("NextRunAt"))
            {
                await connection.ExecuteAsync("ALTER TABLE Jobs ADD COLUMN NextRunAt DATETIME");
            }
            if (!columnNames.Contains("ConfigurationJson"))
            {
                await connection.ExecuteAsync("ALTER TABLE Jobs ADD COLUMN ConfigurationJson TEXT");
            }
            if (!columnNames.Contains("IgnorePatterns"))
            {
                await connection.ExecuteAsync("ALTER TABLE Jobs ADD COLUMN IgnorePatterns TEXT");
            }
        }

        public async Task<IEnumerable<JobMetadata>> GetAllJobsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // Complex query to get Jobs + Latest Run ID + Health Score (based on last 5 runs)
            // Health calculation: (SuccessfulRuns / TotalRunsInWindow) * 100
            // Success = Completed (2) or Skipped (4)
            return await connection.QueryAsync<JobMetadata>(@"
                WITH LastFiveRuns AS (
                    SELECT 
                        JobId,
                        Status,
                        ROW_NUMBER() OVER (PARTITION BY JobId ORDER BY StartTime DESC) as rn
                    FROM JobRuns
                ),
                JobStats AS (
                    SELECT 
                        JobId,
                        COUNT(*) as TotalRuns,
                        SUM(CASE WHEN Status IN (2, 4) THEN 1 ELSE 0 END) as SuccessCount
                    FROM LastFiveRuns
                    WHERE rn <= 5
                    GROUP BY JobId
                ),
                LatestRun AS (
                    SELECT 
                        JobId, 
                        Id as LastRunId, 
                        Status as LastRunStatus,
                        ROW_NUMBER() OVER (PARTITION BY JobId ORDER BY StartTime DESC) as rn
                    FROM JobRuns
                )
                SELECT 
                    j.*,
                    lr.LastRunId,
                    lr.LastRunStatus,
                    COALESCE(js.TotalRuns, 0) as TotalRunsInWindow,
                    COALESCE(js.SuccessCount, 0) as SuccessCountInWindow,
                    CASE 
                        WHEN js.TotalRuns IS NULL OR js.TotalRuns = 0 THEN 100 -- Default to healthy if no runs
                        ELSE CAST((js.SuccessCount * 100.0 / js.TotalRuns) AS INTEGER)
                    END as HealthScore
                FROM Jobs j
                LEFT JOIN JobStats js ON j.Id = js.JobId
                LEFT JOIN LatestRun lr ON j.Id = lr.JobId AND lr.rn = 1
                ORDER BY j.CreatedAt DESC");
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
                INSERT INTO Jobs (Name, Type, Status, CreatedAt, ConfigurationJson, CronExpression, NextRunAt, IgnorePatterns) 
                VALUES (@Name, @Type, @Status, @CreatedAt, @ConfigurationJson, @CronExpression, @NextRunAt, @IgnorePatterns);
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
                    NextRunAt = @NextRunAt,
                    IgnorePatterns = @IgnorePatterns
                WHERE Id = @Id", job);
        }

        // --- Job Run Methods ---

        public async Task<int> AddRunAsync(JobRun run)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleAsync<int>(@"
                INSERT INTO JobRuns (JobId, StartTime, Status, Logs, ResultMessage) 
                VALUES (@JobId, @StartTime, @Status, @Logs, @ResultMessage);
                SELECT last_insert_rowid();",
                run);
        }

        public async Task UpdateRunAsync(JobRun run)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(@"
                UPDATE JobRuns 
                SET EndTime = @EndTime, 
                    Status = @Status, 
                    Logs = @Logs, 
                    ResultMessage = @ResultMessage
                WHERE Id = @Id", run);
        }

        public async Task<IEnumerable<JobRun>> GetRunsByJobIdAsync(int jobId, int limit = 50)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<JobRun>(
                "SELECT * FROM JobRuns WHERE JobId = @JobId ORDER BY StartTime DESC LIMIT @Limit", 
                new { JobId = jobId, Limit = limit });
        }

        public async Task<JobRun?> GetRunByIdAsync(int runId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<JobRun>("SELECT * FROM JobRuns WHERE Id = @Id", new { Id = runId });
        }
    }
}