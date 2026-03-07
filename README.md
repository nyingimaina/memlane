# Memlane v1.0.0

Memlane is a streamlined, rock-solid folder backup and synchronization utility. Built with a .NET 10 backend and a React (Next.js) frontend, it provides an at-a-glance dashboard for managing your critical data protection pipelines.

## Key Features (v1.0.0)
- **Folder Backup & Sync:** Intelligent synchronization using SHA256 hashing to only copy changed files.
- **Job Health Monitoring:** Jenkins-inspired weather icons (Sunny, Cloudy, Stormy) based on job stability.
- **Smart Scheduling:** Full Cron-based scheduling for automated background backups.
- **Ignore Patterns:** Full support for `.memignore` style patterns to exclude logs, temp files, and bulky directories.
- **Retention Management:** Automated rotation of old backups to save disk space.
- **Windows Integration:** Packaged as a standard Windows application via Inno Setup.

## Getting Started (Development)

### Prerequisites
- .NET 10 SDK
- Node.js (v18+)

### Running the App
1. **Start Backend:**
   ```bash
   cd back-end/Memlane.Api
   dotnet run
   ```
2. **Start Frontend:**
   ```bash
   cd front-end
   npm install
   npm run dev
   ```
3. Open `http://localhost:3000`

## Deployment & Installation

Memlane can be built into a standalone Windows installer using the provided scripts in the `deployment/` folder.

1. **Publish:** Run `deployment/publish.ps1` from PowerShell. This will bundle the frontend into the backend's `wwwroot` and publish a self-contained executable.
2. **Pack:** Open `deployment/Memlane.iss` in **Inno Setup** and click 'Compile'.
3. **Install:** Run the generated `MemlaneSetup_v1.0.0.exe`.

## Project Architecture
- **Vertical Slice Architecture:** UI, Logic, and Data for each feature are co-located for maximum maintainability.
- **Modern Tech Stack:** .NET 10, Dapper (SQLite), Next.js 14, SignalR (Real-time updates).

---
*Developed with love for data integrity.*
