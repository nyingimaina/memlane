# Track Specification: Build core backup and synchronization engine

## Overview
This track implements the foundational C# logic for Memlane's backup and synchronization capabilities. It includes a provider-based architecture for database backups, an intelligent file sync engine, and integration with Hangfire for job scheduling and SQLite for metadata storage.

## Objectives
- Implement a robust, extensible backup engine in .NET 8.
- Support initial database providers: SQL Server and MariaDB.
- Support recursive file/folder synchronization using hash-based change detection.
- Provide a persistent job management system using Hangfire and SQLite.
- Ensure all operations are resilient with Polly-based retry policies.

## Key Components
- **Backup Engine:** Orchestrates backup and sync jobs.
- **Database Providers:** `SqlServerBackupProvider`, `MariaDbBackupProvider`.
- **Storage Providers:** `LocalStorageProvider` (initial).
- **Sync Engine:** `FileHashSyncEngine` (recursive, hash-based).
- **Compression:** `CompressionUtility` for GZip/Zip.
- **Job Management:** Hangfire integration.
- **Persistence:** SQLite for metadata and Hangfire state.

## Tech Stack Alignment
- .NET 8 (LTS)
- Dapper
- Hangfire (SQLite storage)
- SignalR (ready for dashboard integration)
- Polly