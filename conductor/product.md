# Initial Concept

A an extensible backup utility that'll allow me to trigger sql server back ups, mariadb backups and also allow compression of files and copy or moving them from one point in disk to another. For folders/files if we can do a hash check to tell if any files recursively changed and only then trigger next back up steps then that would be an added bonus. The application should run on windows and also linux and should be dockerizable. Let's use C# for the backend and NextJs TS for the frontend dashboard. Also we need to be able to run as a windows service and to have cron jobs or similar and also dockerizable on linux

---

# Product Guide: Memlane

## Vision
A versatile, extensible backup and file synchronization utility designed for both individual developers and system administrators. Memlane bridges the gap between simple script-based backups and complex enterprise solutions, offering a powerful core in C# for reliability and a modern Next.js dashboard for ease of use.

## Core Features
- **Extensible Database Backups:** Initial support for SQL Server and MariaDB, with a plugin-ready architecture for future providers.
- **Intelligent File Synchronization:** Advanced hash-based change detection for recursive file/folder backups, ensuring only modified data is processed.
- **Intelligent Optimization:** Automatically skip backup and compression steps when no file changes are detected, with configurable overrides for critical jobs.
- **Cross-Platform & Deployment:** Native support for Windows (running as a service) and Linux (Dockerized), with integrated Cron-like scheduling.
- **Data Compression & Movement:** Seamless compression and transfer of files across different disk locations.
- **Cloud & Remote Integration:** Built-in support for cloud storage (S3, Azure, Google Drive) and a remote-to-local synchronization capability for easy data retrieval.

## User Experience
- **Next.js Dashboard:** A modern, TypeScript-powered interface for real-time monitoring, job configuration, and performance analytics.
- **Reliability First:** Every backup job is verified via hash checks to ensure data integrity.
- **Flexible Management:** Manage backup jobs visually through the dashboard or via a robust configuration layer.

## Target Audience
- **Individual Developers:** Who need a reliable way to backup personal projects and databases locally or to the cloud.
- **System Administrators:** Who require an extensible, cross-platform tool for managing infrastructure backups with centralized monitoring.