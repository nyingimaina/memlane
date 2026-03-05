# Track Specification: Backup Reliability & Pipeline Fix

## Overview
This track addresses critical bugs in the backup pipeline where jobs are incorrectly skipped and artifacts are not being saved to their final destination. It ensures that file change detection is reliable while still producing a fresh artifact for every successful run.

## Objectives
- **Workspace Separation:** Distinguish between the "Sync Workspace" (persistent for hash comparison) and the "Artifact Workspace" (ephemeral for current run output).
- **Dynamic Filename Generation:** Integrate `IFilenameGenerator` to ensure every backup artifact has a unique, timestamped name.
- **Reliable Skip Logic:** Ensure `SkipIfNoChanges` only triggers when truly no data has changed, but a full backup is still produced when changes *are* detected.
- **Path Verification:** Ensure all target destinations are correctly handled by storage providers.

## Key Components
- **`BackupJobOrchestrator` Refactor:** Inject `IFilenameGenerator` and implement dual-workspace logic.
- **`SyncResult` Enhancement:** Provide clearer data on what exactly changed to aid debugging.

## Success Criteria
- The first run of a job always produces a backup artifact in the target destination.
- Subsequent runs produce a new artifact ONLY if files have changed (or DB backup is present).
- Artifacts follow the `[JobName]_Full_[Timestamp]_[Hash].zip` naming convention.
- Target destinations contain the expected backup files.
