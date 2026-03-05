# Track Specification: v1.0.0 Stabilization (Folder Backup Only)

## Overview
Refine the product for its initial v1.0.0 release by focusing exclusively on the core value proposition: **Folder Backup**. This involves simplifying the user interface to reduce confusion and ensure a rock-solid first impression.

## Requirements
- **Hide Database Backup:** Remove or hide all UI elements related to SQL Server or MariaDB backup.
- **Default to Directory:** Ensure all new jobs default to "Directory Backup".
- **Installer Research:** Document and prepare the strategy for a Windows Installer (.exe/.msi).
- **Cleanup:** Ensure all 1.0.0 features are stable and well-documented.

## User Experience
- When a user clicks "Create New Job", they are immediately presented with the Directory Backup configuration.
- No "Database Backup" toggle is visible.
- The UI feels focused and purpose-built for file synchronization.
