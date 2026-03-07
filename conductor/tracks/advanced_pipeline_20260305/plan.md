# Implementation Plan: Advanced Pipeline & 7z Compression

## Phase 1: 7z Backend Integration
### [~] Task: 7z Provider & Execution Logic (DEBUGGING)
- [x] Implement: Create `SevenZipProvider.cs` using `SevenZipSharp.Interop` for robust DLL-based compression.
- [x] Implement: Implement Adapter Pattern with `ICompressionProvider` and `CompressionProviderFactory` to support Zip and 7z.
- [x] Implement: Update `BackupJobOrchestrator` to use dynamic compression providers.
- [ ] Bug: Resolve `7z.dll` loading issues in runtime.

## Phase 2: Refined Pipeline Workflow
### [x] Task: Atomic Move & Cleanup
- [x] Implement: Logic to ensure archives are moved only after successful compression using specific temp folders.
- [x] Implement: Strict temporary file cleanup for both compression workspace and artifact assembly.

## Phase 3: Frontend Enhancements
### [~] Task: Advanced Compression UI & Visibility
- [x] Implement: Update `JobForm` to include 7z-specific settings using Adapter Pattern.
- [x] Implement: Create specialized `ZipOptions` and `SevenZipOptions` components for format-specific settings.
- [x] Refactor: Migrate all UI components to Vertical Slice Architecture with pure CSS Modules and local theme tokens.
- [x] Fix: Resolve visibility and contrast issues in the side pane (forced via `visibilityProvider`).
- [ ] Feature: Add descriptive status text for "Last Run" and summary of recent runs to the health column.
- [ ] Style: Use scary/red colors for failure states in health icons.

## Phase 4: Dashboard Observability
### [ ] Task: Accurate Metrics
- [ ] Bug: Fix "Recent Failures" dashboard metric (currently reporting 0 erroneously).
- [ ] Implement: Refactor dashboard to calculate success/failure rates from run history rather than aggregate state.

- [ ] Task: Conductor - User Manual Verification 'Phase 1: Advanced Pipeline' (Protocol in workflow.md)
