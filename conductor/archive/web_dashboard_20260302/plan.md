# Implementation Plan: Web Dashboard & Core Integration (Full)

## Phase 1: Frontend & Backend Core Stabilized [checkpoint: 35fa89b]
(Completed: Scaffolding, Infrastructure, Basic Layout)

## Phase 2: Pipeline & Storage Implementation

### [ ] Task: Real Storage & Database Providers
- [ ] Implement: `SqlServerBackupProvider` using `Microsoft.Data.SqlClient` (dynamic command generation)
- [ ] Implement: `MariaDbBackupProvider` using `mysqldump` process execution
- [ ] Implement: `S3StorageProvider` (Extensibility stub/interface)
- [ ] Implement: `StorageProviderFactory` to resolve providers by type

### [ ] Task: Enhanced Orchestration Pipeline
- [ ] Implement: Update `BackupJobOrchestrator` to support the full pipeline:
    - [ ] DB Provider Execution -> Output to Temp
    - [ ] Sync Engine (Hashing) -> Optional Skip
    - [ ] Compression Utility -> Archive Creation
    - [ ] Storage Provider -> Move/Copy to Final Destination
- [ ] Implement: Comprehensive logging for each pipeline step

- [ ] Task: Conductor - User Manual Verification 'Phase 2: Pipeline & Storage' (Protocol in workflow.md)

## Phase 3: Full Dashboard & UI Completion

### [ ] Task: Job Configuration & Management (No Placeholders)
- [ ] Implement: Update `JobForm` with Storage Provider selection and Destination paths
- [ ] Implement: Implement `HistoryPage` with a searchable table of previous job runs
- [ ] Implement: Implement `SettingsPage` for global app configuration (e.g., default paths, S3 credentials)
- [ ] Implement: Enhance `DashboardPage` with real-time summary cards (Total Jobs, Success Rate, Storage Used)

### [ ] Task: Final Integration & E2E Verification
- [ ] Implement: Full E2E test verifying a pipeline: SQL -> Sync -> Compress -> Local Storage
- [ ] Implement: Verify Dark/Light mode consistency across all new components
- [ ] Implement: Final build and cleanup of any remaining `console.log` or debug stubs

- [ ] Task: Conductor - User Manual Verification 'Phase 3: Final Integration' (Protocol in workflow.md)