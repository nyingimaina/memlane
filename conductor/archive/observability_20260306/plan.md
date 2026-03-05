# Implementation Plan: Enterprise Observability

## Phase 1: Backend Foundation

### [x] Task: JobRuns Schema & Persistence
- [x] Implement: Add `JobRuns` table to SQLite schema in `IJobRepository`.
- [x] Implement: Create `JobRun` model and update repository with `AddRunAsync` and `UpdateRunAsync`.

### [x] Task: Verbose Orchestrator Logging
- [x] Implement: Refactor `BackupJobOrchestrator` to create a `JobRun` at start.
- [x] Implement: Add an `ILogger`-like buffer to the orchestrator that collects verbose messages and flushes them to the `JobRuns` table and SignalR.
- [x] Implement: Add detailed logging to `FileHashSyncEngine`, `BackupProviders`, and `StorageProviders`.

## Phase 2: Frontend & API Integration

### [x] Task: History API Endpoints
- [x] Implement: Add `GET /api/jobs/{id}/runs` and `GET /api/runs/{id}` endpoints.

### [x] Task: Jenkins-style History UI
- [x] Implement: Create `JobHistoryList` and `RunLogViewer` components.
- [x] Implement: Integrate history view into the existing dashboard or a dedicated history page.
- [x] Implement: Add a "Live Console" view for active jobs using SignalR.

- [ ] Task: Conductor - User Manual Verification 'Phase 2: Observability' (Protocol in workflow.md)
