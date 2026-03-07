# Implementation Plan: Advanced Pipeline & 7z Compression

## Phase 1: 7z Backend Integration
### [x] Task: 7z Provider & Execution Logic
- [x] Implement: Create `SevenZipCommandLineProvider.cs` using `7-Zip.CommandLine` NuGet for cross-platform support.
- [x] Implement: Add configuration options for compression levels (Fast, Normal, Ultra).
- [x] Implement: Update `BackupJobOrchestrator` to use the new `ICompressionProvider` and `.7z` extension.

## Phase 2: Refined Pipeline Workflow
### [x] Task: Atomic Move & Cleanup
- [x] Implement: Logic to ensure archives are moved only after successful compression using specific temp folders.
- [x] Implement: Strict temporary file cleanup for both compression workspace and artifact assembly.

## Phase 3: Frontend Enhancements
### [x] Task: Advanced Compression UI
- [x] Implement: Update `JobForm` to include 7z-specific settings (Compression Level dropdown).
- [x] Implement: Real-time progress reporting for the compression phase via SignalR (integrated into existing flow).

- [ ] Task: Conductor - User Manual Verification 'Phase 1: Advanced Pipeline' (Protocol in workflow.md)
