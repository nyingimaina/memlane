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
}
