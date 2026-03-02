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
    }
}