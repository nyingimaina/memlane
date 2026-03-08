# Implementation Plan: Advanced Pipeline & 7z Compression

## Phase 1: High-Ratio Backend Integration
### [x] Task: Zstandard Provider & Execution Logic
- [x] Implement: Create `ZstdProvider.cs` using `ZstdSharp.Port` for pure C# cross-platform compression.
- [x] Implement: Implement Adapter Pattern with `ICompressionProvider` and `CompressionProviderFactory` to support Zip and Zstd.
- [x] Implement: Update `BackupJobOrchestrator` to use dynamic compression providers.
- [x] Bug: Resolved `7z.dll` loading issues by switching to a dependency-free Zstd implementation.

## Phase 2: Refined Pipeline Workflow
### [x] Task: Atomic Move & Cleanup
- [x] Implement: Logic to ensure archives are moved only after successful compression using specific temp folders.
- [x] Implement: Strict temporary file cleanup for both compression workspace and artifact assembly.

## Phase 3: Frontend Enhancements
### [x] Task: Advanced Compression UI & Visibility
- [x] Implement: Update `JobForm` to include Zstandard settings using Adapter Pattern.
- [x] Implement: Create specialized `ZipOptions` and `ZstdOptions` components for format-specific settings.
- [x] Refactor: Migrate all UI components to Vertical Slice Architecture with pure CSS Modules and local theme tokens.
- [x] Fix: Resolve visibility and contrast issues in the side pane (forced via `visibilityProvider`).
- [x] Feature: Added descriptive status text for "Last Run" and summary of recent runs to the health column.
- [x] Style: Used scary/red colors for failure states in health icons.

## Phase 4: Dashboard Observability
### [x] Task: Accurate Metrics
- [x] Bug: Fixed "Recent Failures" dashboard metric (now correctly reporting based on last run history).
- [x] Implement: Refactored dashboard to calculate success/failure rates from run history rather than aggregate state.

- [x] Task: Conductor - User Manual Verification 'Phase 1: Advanced Pipeline' (Verified by developer)

