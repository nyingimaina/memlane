namespace Memlane.Api.Models
{
    public enum JobStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        Skipped
    }

    public class JobMetadata
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // e.g., "Backup", "Sync"
        public JobStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastRunAt { get; set; }
        public string? LastError { get; set; }
        public string? ConfigurationJson { get; set; }
        public string? CronExpression { get; set; }
        public DateTime? NextRunAt { get; set; }

        // Calculated / Joined fields for Monitoring
        public int? LastRunId { get; set; }
        public JobStatus? LastRunStatus { get; set; }
        public int HealthScore { get; set; } // 0 to 100
        public int TotalRunsInWindow { get; set; }
        public int SuccessCountInWindow { get; set; }
    }

    public class JobRun
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public JobStatus Status { get; set; }
        public string? Logs { get; set; }
        public string? ResultMessage { get; set; }
    }
}