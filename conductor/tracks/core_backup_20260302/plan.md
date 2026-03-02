# Implementation Plan: Build core backup and synchronization engine

## Phase 1: Infrastructure & Core Abstractions

### [x] Task: Project Scaffolding (3f52e8f)
- [x] Write Tests: Verify project setup and dependency resolution
- [x] Implement: Create .NET 8 Web API project and configure basic middleware
- [x] Implement: Setup SQLite connection and Dapper configuration

### [ ] Task: Persistence & Job Management Setup
- [ ] Write Tests: Verify SQLite database connectivity and schema creation
- [ ] Implement: Configure Hangfire with SQLite storage
- [ ] Implement: Basic job registration and execution logic

### [ ] Task: Core Interfaces & Resilience
- [ ] Write Tests: Verify `IBackupProvider` and `IStorageProvider` contract behavior (via mocks)
- [ ] Implement: Define `IBackupProvider` and `IStorageProvider` interfaces
- [ ] Implement: Setup Polly retry policies for transient error handling

- [ ] Task: Conductor - User Manual Verification 'Phase 1: Infrastructure & Core Abstractions' (Protocol in workflow.md)

## Phase 2: Provider Implementation & Sync Logic

### [ ] Task: Database Backup Providers
- [ ] Write Tests: Verify `SqlServerBackupProvider` (mocking SQL Server commands)
- [ ] Write Tests: Verify `MariaDbBackupProvider` (mocking MariaDB commands)
- [ ] Implement: `SqlServerBackupProvider` logic
- [ ] Implement: `MariaDbBackupProvider` logic

### [ ] Task: File Synchronization Engine
- [ ] Write Tests: Verify recursive directory scanning and hash generation
- [ ] Write Tests: Verify sync logic (copy only modified files)
- [ ] Implement: `FileHashSyncEngine` with recursive hash checking

### [ ] Task: Compression & Storage
- [ ] Write Tests: Verify file compression/decompression logic
- [ ] Write Tests: Verify `LocalStorageProvider` behavior
- [ ] Implement: `CompressionUtility` (Zip/GZip)
- [ ] Implement: `LocalStorageProvider` for file movement

- [ ] Task: Conductor - User Manual Verification 'Phase 2: Provider Implementation & Sync Logic' (Protocol in workflow.md)

## Phase 3: Job Orchestration & Final Integration

### [ ] Task: Backup Job Orchestration
- [ ] Write Tests: Verify `BackupJob` orchestrates providers correctly
- [ ] Implement: `BackupJob` to coordinate backup, sync, and compression steps

### [ ] Task: Final Integration & CLI Runner
- [ ] Write Tests: Verify job execution from CLI/Service runner
- [ ] Implement: Basic CLI command or background service to trigger jobs
- [ ] Implement: SignalR hub for real-time progress updates

- [ ] Task: Conductor - User Manual Verification 'Phase 3: Job Orchestration & Final Integration' (Protocol in workflow.md)