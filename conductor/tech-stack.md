# Tech Stack: Memlane

## Backend (C#)
- **Runtime:** .NET 8 (LTS) - For stability and long-term support.
- **Framework:** ASP.NET Core Web API - To provide a robust RESTful interface.
- **Real-time:** SignalR - For live status updates on the Next.js dashboard.
- **ORM:** Dapper - For high-performance, lightweight database access.
- **Scheduling & Job Management:** Built-in .NET `BackgroundService` (with SQLite persistence and Polly retries) - To manage background backup tasks with built-in persistence and retries.
- **Resilience:** Polly - For implementing advanced retry policies and circuit breakers.
- **Abstractions:** A provider-based architecture for database backups (SQL Server, MariaDB) and storage (Local, S3, Azure, Google Drive).

## Frontend (Next.js)
- **Framework:** Next.js (TypeScript) - For a modern, type-safe dashboard.
- **State Management:** Logic-Repository pattern (as per Scholarity project context).
- **Styling:** Vanilla CSS - For a clean, corporate, and precise UI.
- **Communication:** Axios/Fetch for REST API and SignalR client for real-time updates.

## Storage & Database
- **Metadata Storage:** SQLite - A zero-configuration, file-based database for job history, settings, and background job state.
- **Target Databases:** Support for SQL Server and MariaDB (initially).
- **Compression:** Standard libraries (e.g., `System.IO.Compression`) for ZIP/GZIP support.

## Infrastructure & Deployment
- **Windows:** Support for running as a Windows Service.
- **Linux:** Fully Dockerizable for containerized environments.
- **Scheduling:** Integrated .NET background service, with hooks for system-native Cron if needed.