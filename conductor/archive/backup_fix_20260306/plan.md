# Implementation Plan: Backup Reliability & Pipeline Fix

## Phase 1: Core Pipeline Refactor

### [x] Task: Orchestrator Refactor (Workspace Separation)
- [x] Implement: Inject `IFilenameGenerator` into `BackupJobOrchestrator`.
- [x] Implement: Refactor `ExecuteJobAsync` to use a persistent `syncWorkspace` for `FileHashSyncEngine`.
- [x] Implement: Use a fresh `artifactWorkspace` for each run to gather DB dumps and files.
- [x] Implement: Generate dynamic `ArchiveFileName` using the generator.
- [x] Implement: Ensure `SkipIfNoChanges` is correctly calculated based on both DB and file changes.

### [x] Task: Verification & Testing
- [x] Implement: Add unit tests to `EndToEndTests.cs` specifically verifying that a second run WITHOUT changes is skipped, but a second run WITH changes produces a new file.
- [x] Implement: Verify that `LocalStorageProvider` and `FolderStorageProvider` correctly create target directories.

- [ ] Task: Conductor - User Manual Verification 'Phase 1: Pipeline Fix' (Protocol in workflow.md)
