# Implementation Plan: Advanced Job Monitoring

## Phase 1: Backend Foundation

### [x] Task: Health & History API Updates
- [x] Implement: Update `JobMetadata` model to include `LastRunId`, `HealthScore`, and `LastRunStatus`.
- [x] Implement: Update `SqliteJobRepository.GetAllJobsAsync` to join with the latest run and calculate a health score based on the last 5 runs.
- [x] Implement: Ensure the API returns these new calculated fields.

## Phase 2: Frontend Visualization

### [x] Task: Enhanced JobsTable (557e7c2)
- [x] Implement: Add "Health", "Last Run #", and "Next Run" columns to `JobsTable.tsx`
- [x] Implement: Create a `JobHealthIcon` component that maps the health score to weather icons.
- [x] Implement: Add tooltips or secondary text to explain the health score (e.g., "4/5 recent runs successful").

### [x] Task: Real-time UI Refresh (950ec03)
- [x] Implement: Update `useJobLogic.ts` to trigger a full job list refresh when a SignalR completion event is received.
- [x] Implement: Ensure dashboard and jobs table update without manual refresh after a run finishes.

### [x] Task: Job-specific Ignore Patterns (950ec03)
- [x] Implement: Add `IgnorePatterns` (TEXT) to SQLite schema and `JobMetadata` model.
- [x] Implement: Add multiline textarea to `JobForm.tsx` for `.memignore` style patterns.
- [x] Implement: Update `FileHashSyncEngine.cs` to filter files using regex-based glob matching for ignore patterns.

- [ ] Task: Conductor - User Manual Verification 'Phase 2: Monitoring' (Protocol in workflow.md)
