# Initial Concept

A an extensible backup utility that'll allow me to trigger sql server back ups, mariadb backups and also allow compression of files and copy or moving them from one point in disk to another. For folders/files if we can do a hash check to tell if any files recursively changed and only then trigger next back up steps then that would be an added bonus. The application should run on windows and also linux and should be dockerizable. Let's use C# for the backend and NextJs TS for the frontend dashboard. Also we need to be able to run as a windows service and to have cron jobs or similar and also dockerizable on linux

---

# Product Guide: Memlane

## Vision
A versatile, extensible backup and file synchronization utility designed for both individual developers and system administrators. Memlane bridges the gap between simple script-based backups and complex enterprise solutions, offering a powerful core in C# for reliability and a modern Next.js dashboard for ease of use.

## Core Features (v1.0.0 Stabilized)
- **Core Folder Backup & Sync:** Advanced hash-based change detection for recursive file/folder backups, ensuring only modified data is processed.
- **Intelligent Optimization:** Automatically skip backup and compression steps when no file changes are detected, with configurable overrides for critical jobs.
- **Jenkins-style Monitoring:** Real-time health tracking with weather-inspired stability icons and detailed execution logs.
- **Ignore Patterns:** Full support for `.memignore` style patterns to exclude logs, temp files, and bulky directories.
- **Extensible Architecture:** A plugin-ready backend for future database providers (SQL Server, MariaDB), currently focused on rock-solid folder synchronization.
- **Cross-Platform & Deployment:** Native support for Windows (running as a service) and Linux (Dockerized), with integrated Cron-like scheduling.
- **Cloud & Remote Integration:** Built-in support for cloud storage (S3, Azure, Google Drive).

## User Experience
- **Next.js Dashboard:** A modern, TypeScript-powered interface for real-time monitoring, job configuration, and performance analytics.
- **Deep Observability:** Enterprise-grade visibility with persistent, timestamped execution logs for every run. The "Jenkins-style" console reveals exactly which files were scanned, changed, or skipped, providing absolute peace of mind and rapid debuggability.
- **Premium Onboarding:** Interactive guided tours using `react-joyride` to introduce new users to the platform and explain complex configuration options.
- **Guided Configuration:** Streamlined job creation with interactive directory pickers and a user-friendly Cron builder to minimize keyboard entry.
- **Reliability First:** Every backup job is verified via hash checks to ensure data integrity.
- **Flexible Management:** Manage backup jobs visually through the dashboard or via a robust configuration layer.

## Target Audience
- **Individual Developers:** Who need a reliable way to backup personal projects locally or to the cloud.
- **System Administrators:** Who require an extensible, cross-platform tool for managing infrastructure backups with centralized monitoring.
