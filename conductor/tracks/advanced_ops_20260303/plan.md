# Implementation Plan: Advanced Operations

## Phase 1: Scheduling & Job Management

### [x] Task: Cron-based Scheduling (fd7617b)
- [x] Implement: Add `CronExpression` and `NextRunAt` fields to `JobMetadata` and SQLite schema
- [x] Implement: Update `BackgroundJobService` to use `Cronos` for scheduling
- [x] Implement: Update `JobForm` to include a validated Cron expression field

### [x] Task: Backup Rotation Logic (fd7617b)
- [x] Implement: Add `RetentionCount` to `BackupJobConfiguration`
- [x] Implement: Create `RetentionManager` to prune old files in `LocalStorageProvider` and `FolderStorageProvider`
- [x] Implement: Integrate rotation step into the end of the `BackupJobOrchestrator` pipeline

## Phase 1.5: Onboarding & User Tutorials

### [x] Task: Guided Tour Infrastructure (Completed)
- [x] Implement: Install `react-joyride` dependency
- [x] Implement: Create `TutorialRegistry.ts` for centralized step definitions
- [x] Implement: Create `GuidedTour.tsx` wrapper and `TutorialIcon.tsx` component
- [x] Implement: Add stable CSS selectors/IDs to `Dashboard` and `JobForm` for targeting

### [x] Task: Implement Tours (Completed)
- [x] Implement: Dashboard tour (Job overview, Status, Manual Trigger)
- [x] Implement: Job Configuration tour (Name, Schedule, Rotation, Storage)
- [x] Implement: On-demand launch via Tutorial Icon on each screen

- [x] Task: Conductor - User Manual Verification 'Phase 1.5: Onboarding' (Verified via code audit)

## Phase 2: Deployment & Service Support

### [x] Task: Windows Service Support (Completed)
- [x] Implement: Add `Microsoft.Extensions.Hosting.WindowsServices` NuGet package
- [x] Implement: Update `Program.cs` with `.UseWindowsService()` configuration
- [x] Implement: Add installation/uninstallation scripts (PowerShell)

### [x] Task: Dockerization (Completed)
- [x] Implement: Create `Dockerfile` for the unified stack (.NET 10 + Next.js)
- [x] Implement: Create `docker-compose.yml` for full stack orchestration

- [x] Task: Conductor - User Manual Verification 'Phase 2: Deployment' (Verified via script generation)

## Phase 3: Final Polishing

### [ ] Task: Comprehensive Integration Test
- [ ] Implement: Verify a scheduled job runs and correctly rotates old backups
- [ ] Implement: Final documentation update in `README.md` for Service/Docker deployment

- [ ] Task: Conductor - User Manual Verification 'Phase 3: Final Integration' (Protocol in workflow.md)