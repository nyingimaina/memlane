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

### [ ] Task: Guided Tour Infrastructure
- [ ] Implement: Install `react-joyride` dependency
- [ ] Implement: Create `TutorialRegistry.ts` for centralized step definitions
- [ ] Implement: Create `GuidedTour.tsx` wrapper and `TutorialIcon.tsx` component
- [ ] Implement: Add stable CSS selectors/IDs to `Dashboard` and `JobForm` for targeting

### [ ] Task: Implement Tours
- [ ] Implement: Dashboard tour (Job overview, Status, Manual Trigger)
- [ ] Implement: Job Configuration tour (Name, Schedule, Rotation, Storage)
- [ ] Implement: On-demand launch via Tutorial Icon on each screen

- [ ] Task: Conductor - User Manual Verification 'Phase 1.5: Onboarding' (Protocol in workflow.md)

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