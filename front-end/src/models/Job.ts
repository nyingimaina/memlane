export enum JobStatus {
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3,
    Skipped = 4
}

export interface JobMetadata {
    id: number;
    name: string;
    type: string;
    status: JobStatus;
    createdAt: string;
    lastRunAt?: string;
    lastError?: string;
    configurationJson?: string;
    cronExpression?: string;
    nextRunAt?: string;
    ignorePatterns?: string;

    // Calculated / Joined fields for Monitoring
    lastRunId?: number;
    lastRunStatus?: JobStatus;
    healthScore: number; // 0 to 100
    totalRunsInWindow: number;
    successCountInWindow: number;
}

export interface JobRun {
    id: number;
    jobId: number;
    startTime: string;
    endTime?: string;
    status: JobStatus;
    logs?: string;
    resultMessage?: string;
}

export interface JobStatusUpdate {
    jobId: number;
    status: string;
    message: string;
    progressPercentage: number;
}

export interface BackupJobConfiguration {
    dbProvider?: string;
    dbConnectionString?: string;
    sourceDirectory?: string;
    storageProvider?: string;
    targetDestination?: string;
    enableCompression: boolean;
    archiveFileName?: string;
    skipIfNoChanges: boolean;
    retentionCount: number;
    compressionLevel?: "Fastest" | "Fast" | "Normal" | "Maximum" | "Ultra";
}
