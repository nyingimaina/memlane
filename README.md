# Memlane Backup Utility

Memlane is an extensible backup and synchronization utility with a .NET 8 backend and a Next.js frontend.

## Getting Started (Backend)

### Prerequisites
- .NET 8 SDK

### Running the API
1. Navigate to the root directory.
2. Run the following command:
   ```bash
   dotnet run --project back-end/Memlane.Api/Memlane.Api.csproj
   ```
3. The API will start and initialize a local SQLite database (`memlane.db`).

### Manual Verification (Phase 2)

Since the dashboard is not yet implemented, you can verify Phase 2 logic using the provided test suite or by observing logs during background job execution.

#### 1. Run Automated Tests
Verify all core logic (Providers, Sync Engine, Compression) by running:
```bash
dotnet test
```

#### 2. Verify Database Providers (Simulation)
The `SqlServerBackupProvider` and `MariaDbBackupProvider` currently simulate file generation. You can see their behavior in `ProviderTests.cs`.

#### 3. Verify File Synchronization
The `FileHashSyncEngine` uses SHA256 hashing. You can verify its behavior by:
1. Creating a `source` and `target` directory.
2. Placing files in `source`.
3. The engine (triggered via tests or future API) will only copy modified files based on hash changes.

#### 4. Verify Compression
The `CompressionUtility` can be verified via `StorageTests.cs`, which confirms that directories are correctly zipped and extracted.

## Project Structure
- `back-end/`: .NET 8 Web API and xUnit Tests.
- `conductor/`: Project configuration, implementation plans, and tracks.
