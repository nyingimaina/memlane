# Implementation Plan: Build core backup and synchronization engine

## Phase 1: Infrastructure & Core Abstractions [checkpoint: 5a8658e]

### [x] Task: Project Scaffolding (3f52e8f)
- [x] Write Tests: Verify project setup and dependency resolution
- [x] Implement: Create .NET 8 Web API project and configure basic middleware
- [x] Implement: Setup SQLite connection and Dapper configuration

### [x] Task: Persistence & Job Management Setup (0e116c1)
- [x] Write Tests: Verify SQLite database connectivity and schema creation
- [x] Implement: Configure built-in background service (IHostedService/BackgroundService)
- [x] Implement: Setup basic job registration and execution logic with Polly retries

### [x] Task: Core Interfaces & Resilience (82a4472)
- [x] Write Tests: Verify `IBackupProvider` and `IStorageProvider` contract behavior (via mocks)
- [x] Implement: Define `IBackupProvider` and `IStorageProvider` interfaces
- [x] Implement: Setup Polly retry policies for transient error handling

- [x] Task: Conductor - User Manual Verification 'Phase 1: Infrastructure & Core Abstractions' (Protocol in workflow.md) (5a8658e)

## Phase 2: Provider Implementation & Sync Logic [checkpoint: 2744c8b]

### [x] Task: Database Backup Providers (cbf0cd0)
- [x] Write Tests: Verify `SqlServerBackupProvider` (mocking SQL Server commands)
- [x] Write Tests: Verify `MariaDbBackupProvider` (mocking MariaDB commands)
- [x] Implement: `SqlServerBackupProvider` logic
- [x] Implement: `MariaDbBackupProvider` logic

### [x] Task: File Synchronization Engine (7d3ce3f)
- [x] Write Tests: Verify recursive directory scanning and hash generation
- [x] Write Tests: Verify sync logic (copy only modified files)
- [x] Implement: `FileHashSyncEngine` with recursive hash checking

### [x] Task: Compression & Storage (4c44e93)
- [x] Write Tests: Verify file compression/decompression logic
- [x] Write Tests: Verify `LocalStorageProvider` behavior
- [x] Implement: `CompressionUtility` (Zip/GZip)
- [x] Implement: `LocalStorageProvider` for file movement

- [x] Task: Conductor - User Manual Verification 'Phase 2: Provider Implementation & Sync Logic' (Protocol in workflow.md) (2744c8b)

## Phase 3: Job Orchestration & Final Integration

### [x] Task: Backup Job Orchestration (feab0b0)
- [x] Write Tests: Verify `BackupJob` orchestrates providers correctly
- [x] Implement: `BackupJob` to coordinate backup, sync, and compression steps

### [x] Task: Final Integration & CLI Runner (e43f60f)
- [x] Write Tests: Verify job execution from CLI/Service runner
- [x] Implement: Basic CLI command or background service to trigger jobs
- [x] Implement: SignalR hub for real-time progress updates

- [ ] Task: Conductor - User Manual Verification 'Phase 3: Job Orchestration & Final Integration' (Protocol in workflow.md)