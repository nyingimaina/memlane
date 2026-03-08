# Track Specification: Advanced Operations (Scheduling, Rotation, Deployment)

## Overview
This track completes the operational lifecycle of Memlane by adding autonomous scheduling, storage management via rotation, and cross-platform deployment support. Users will be able to schedule jobs using Cron expressions, limit the number of stored backups, and run the utility as a native Windows service or a Docker container.

## Objectives
- **Scheduling:** Implement a robust scheduler in the C# backend that executes jobs based on user-defined Cron expressions.
- **Backup Rotation:** Implement logic to prune old backups in Local/Folder destinations based on a configurable `RetentionCount`.
- **Windows Service:** Configure the .NET 8 API to run as a Windows Service using `Microsoft.Extensions.Hosting.WindowsServices`.
- **Dockerization:** Provide a production-ready `Dockerfile` and `docker-compose.yml` for Linux and other containerized environments.
- **UI Updates:** Enhance the `JobForm` to support scheduling and rotation settings.

## Key Components
- **`Cronos` Library:** For parsing and calculating next run times from Cron expressions.
- **`RetentionManager`:** A new service to handle file pruning in backup destinations.
- **`BackgroundJobService`:** Updated to trigger jobs based on schedule rather than just manual "Pending" status.
- **Deployment Artifacts:** `Dockerfile`, `.dockerignore`, `docker-compose.yml`.

## Success Criteria
- Jobs automatically trigger at the times specified by their Cron strings.
- Only the N most recent backups are kept in local/folder destinations when rotation is enabled.
- The application can be installed and started as a Windows Service.
- The application builds and runs successfully within a Docker container.