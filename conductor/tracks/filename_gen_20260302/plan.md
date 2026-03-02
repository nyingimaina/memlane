# Implementation Plan: Implement Dynamic Filename Generation and Change-Aware Skipping

## Phase 1: Dynamic Filename Generation

### [x] Task: Filename Generator Service (e5c446e)
- [x] Write Tests: Verify sortable pattern generation and hash uniqueness
- [x] Implement: `IFilenameGenerator` and `SortableFilenameGenerator`
- [x] Implement: Integration into existing `IBackupProvider` logic

- [ ] Task: Conductor - User Manual Verification 'Phase 1: Dynamic Filename Generation' (Protocol in workflow.md)

## Phase 2: Change Detection & Skipping Logic

### [ ] Task: Enhanced Change Detection
- [ ] Write Tests: Verify `FileHashSyncEngine` correctly reports if changes occurred
- [ ] Implement: Update `ISyncEngine` to return `SyncResult` (or `bool`)
- [ ] Implement: Refactor `FileHashSyncEngine` to track if any file was copied/updated

### [ ] Task: Orchestrator Skip Logic
- [ ] Write Tests: Verify `BackupJobOrchestrator` skips tasks when no changes detected and `SkipIfNoChanges` is true
- [ ] Write Tests: Verify `BackupJobOrchestrator` DOES NOT skip when `SkipIfNoChanges` is false
- [ ] Implement: Add `SkipIfNoChanges` to `BackupJobConfiguration`
- [ ] Implement: Add skip check and logging to `BackupJobOrchestrator`
- [ ] Implement: Ensure "Skipped" status is reported via SignalR and logged to SQLite

- [ ] Task: Conductor - User Manual Verification 'Phase 2: Change Detection & Skipping Logic' (Protocol in workflow.md)

## Phase 3: Final Integration & Refinement

### [ ] Task: Integration & End-to-End Test
- [ ] Write Tests: E2E test verifying a "No Change" job results in no new files and correct logging
- [ ] Implement: Final wiring and cleanup of provider calls
- [ ] Implement: Update `README.md` with new verification steps

- [ ] Task: Conductor - User Manual Verification 'Phase 3: Final Integration & Refinement' (Protocol in workflow.md)