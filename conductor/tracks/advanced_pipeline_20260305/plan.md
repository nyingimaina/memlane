# Implementation Plan: Advanced Pipeline & 7z Compression

## Phase 1: 7z Backend Integration
### [x] Task: 7z Provider & Execution Logic
- [x] Implement: Create `SevenZipCommandLineProvider.cs` using `7-Zip.CommandLine` NuGet for cross-platform support.
- [x] Implement: Add configuration options for compression levels (Fast, Normal, Ultra).
- [x] Implement: Update `BackupJobOrchestrator` to use the new `ICompressionProvider` and `.7z` extension.

## Phase 2: Refined Pipeline Workflow
### [ ] Task: Atomic Move & Cleanup
- [ ] Implement: Logic to ensure archives are moved only after successful compression.
- [ ] Implement: Strict temporary file cleanup to prevent disk bloat.

## Phase 3: Frontend Enhancements
### [ ] Task: Advanced Compression UI
- [ ] Implement: Update `JobForm` to include 7z-specific settings.
- [ ] Implement: Real-time progress reporting for the compression phase via SignalR.
