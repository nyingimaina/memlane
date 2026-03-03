# Implementation Plan: Advanced Operations

## Phase 1: Scheduling & Job Management

### [~] Task: Cron-based Scheduling
- [ ] Implement: Add `CronExpression` and `NextRunAt` fields to `JobMetadata` and SQLite schema
- [ ] Implement: Update `BackgroundJobService` to use `Cronos` for scheduling
- [ ] Implement: Update `JobForm` to include a validated Cron expression field

### [~] Task: Backup Rotation Logic
- [ ] Implement: Add `RetentionCount` to `BackupJobConfiguration`
- [ ] Implement: Create `RetentionManager` to prune old files in `LocalStorageProvider` and `FolderStorageProvider`
- [ ] Implement: Integrate rotation step into the end of the `BackupJobOrchestrator` pipeline

- [ ] Task: Conductor - User Manual Verification 'Phase 1: Scheduling & Rotation' (Protocol in workflow.md)

## Phase 2: Deployment & Service Support

### [ ] Task: Windows Service Support
- [ ] Implement: Add `Microsoft.Extensions.Hosting.WindowsServices` NuGet package
- [ ] Implement: Update `Program.cs` with `.UseWindowsService()` configuration
- [ ] Implement: Add installation/uninstallation scripts (PowerShell)

### [ ] Task: Dockerization
- [ ] Implement: Create `Dockerfile` for the .NET 8 backend
- [ ] Implement: Create `Dockerfile` for the Next.js frontend
- [ ] Implement: Create `docker-compose.yml` for full stack orchestration

- [ ] Task: Conductor - User Manual Verification 'Phase 2: Deployment' (Protocol in workflow.md)

## Phase 3: Final Polishing

### [ ] Task: Comprehensive Integration Test
- [ ] Implement: Verify a scheduled job runs and correctly rotates old backups
- [ ] Implement: Final documentation update in `README.md` for Service/Docker deployment

- [ ] Task: Conductor - User Manual Verification 'Phase 3: Final Integration' (Protocol in workflow.md)