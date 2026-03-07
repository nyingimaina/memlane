# Implementation Plan: Advanced Pipeline & 7z Compression

## Phase 1: 7z Backend Integration
### [x] Task: 7z Provider & Execution Logic
- [x] Implement: Create `SevenZipProvider.cs` using `SevenZipSharp.Interop` for robust DLL-based compression.
- [x] Implement: Implement Adapter Pattern with `ICompressionProvider` and `CompressionProviderFactory` to support Zip and 7z.
- [x] Implement: Update `BackupJobOrchestrator` to use dynamic compression providers.

## Phase 2: Refined Pipeline Workflow
### [x] Task: Atomic Move & Cleanup
- [x] Implement: Logic to ensure archives are moved only after successful compression using specific temp folders.
- [x] Implement: Strict temporary file cleanup for both compression workspace and artifact assembly.

## Phase 3: Frontend Enhancements
### [x] Task: Advanced Compression UI
- [x] Implement: Update `JobForm` to include 7z-specific settings (Compression Level dropdown).
- [x] Implement: Real-time progress reporting for the compression phase via SignalR (integrated into existing flow).

- [ ] Task: Conductor - User Manual Verification 'Phase 1: Advanced Pipeline' (Protocol in workflow.md)
