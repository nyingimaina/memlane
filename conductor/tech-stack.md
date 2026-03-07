# Tech Stack: Memlane

## Backend (C#)
- **Runtime:** .NET 10 - For modern performance and latest features.
- **Framework:** ASP.NET Core Web API - To provide a robust RESTful interface.
- **Real-time:** SignalR - For live status updates on the Next.js dashboard.
- **ORM:** Dapper - For high-performance, lightweight database access.
- **Scheduling & Job Management:** Built-in .NET `BackgroundService` (with SQLite persistence and Polly retries) - To manage background backup tasks with built-in persistence and retries.
- **Resilience:** Polly - For implementing advanced retry policies and circuit breakers.
- **Abstractions:** A provider-based architecture for database backups (SQL Server, MariaDB) and storage (Local, S3, Azure, Google Drive).

## Frontend (Next.js)
- **Framework:** Next.js (TypeScript) - For a modern, type-safe dashboard.
- **Architecture:** Vertical Slice Architecture - Features are self-contained in `src/features/<FeatureName>` with local UI, Logic, Data, and Styles.
- **State Management:** Logic-Repository pattern (as per Scholarity project context).
- **Onboarding:** `react-joyride` - For interactive guided tours and user onboarding.
- **Browser APIs:** File System Access API (with shims) - For native directory picking and file selection.
- **UI Components:** `jattac.libs.web.zest-*` suite.
- **Styling:** Vanilla CSS + CSS Modules - For precise, self-contained component styling.
- **Communication:** Axios/Fetch for REST API and SignalR client for real-time updates.

## Storage & Database
- **Metadata Storage:** SQLite - A zero-configuration, file-based database for job history, settings, and background job state.
- **Target Databases:** Support for SQL Server and MariaDB (plugin-ready).
- **Compression:** Standard libraries (e.g., `System.IO.Compression`) for ZIP/GZIP support.

## Infrastructure & Deployment
- **Windows:** Support for running as a Windows Service. Packaged via Inno Setup.
- **Linux:** Fully Dockerizable for containerized environments.
- **Scheduling:** Integrated .NET background service, with hooks for system-native Cron if needed.
