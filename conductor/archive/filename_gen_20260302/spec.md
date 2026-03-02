# Track Specification: Implement Dynamic Filename Generation and Change-Aware Skipping

## Overview
This track enhances Memlane with a structured naming convention for backup files and an optimization to skip backup steps if no data has changed. It introduces a `IFilenameGenerator` service and modifies the `FileHashSyncEngine` and `BackupJobOrchestrator` to detect and respond to "no-change" scenarios.

## Objectives
- Implement `IFilenameGenerator` following the Sortable Context pattern: `{Source}_Full_{Timestamp}_{ShortHash}`.
- Update `FileHashSyncEngine` to detect if any files were modified during the sync process.
- Enhance `BackupJobOrchestrator` to skip subsequent backup/compression steps and log the event if no changes are detected, **unless the job is configured to force backups**.
- Ensure all filenames are unique and descriptive for both database and file-based backups.

## Key Components
- **`IFilenameGenerator`:** Generates unique, sortable strings for filenames.
- **`SyncResult`:** A new record to return status (ChangesDetected, FilesProcessed, etc.) from the sync engine.
- **`BackupJobConfiguration`:** Enhanced with a `SkipIfNoChanges` boolean flag (defaulting to `true`).
- **`BackupJobOrchestrator`:** Updated logic to check `SyncResult` and `SkipIfNoChanges` before proceeding.

## Success Criteria
- Filenames follow the pattern: `MyDatabase_Full_20260302_234501_a1b2.bak`.
- If a sync is run on a folder with no changed files, the orchestrator logs "No changes detected, skipping backup" and stops the job (if `SkipIfNoChanges` is true).
- Full automated test coverage for filename generation and skip logic.