# Track Specification: Enhanced Job Creation & Onboarding

## Overview
Based on user feedback, the current `JobForm` is confusing. This track refines the UI to clearly distinguish between Database and Directory backups, provides connection string templates, and introduces a user-friendly Cron builder.

## Objectives
- **Job Type Distinction:** Use tabs or a distinct selection at the start of `JobForm` to choose between "DB Backup" and "Directory Backup."
- **Connection String Templates:** Pre-populate the `DbConnectionString` field with provider-specific templates (e.g., `Server=...;Database=...;`) when a DB provider is selected.
- **User-friendly Cron Builder:** Replace or augment the raw Cron text input with a simpler selection (e.g., "Daily at...", "Weekly on...", "Custom").
- **Enhanced Guided Tour:** Update `GuidedTour` to walk users through these new UI elements.

## Key Components
- **`JobForm` Redesign:** Refactor to use a segmented control or tabs for job types.
- **`ConnectionStringTemplates.ts`:** Centralized map of provider-to-template strings.
- **`CronBuilder` Component:** A new UI component to generate Cron expressions visually.

## Success Criteria
- Users can clearly distinguish between DB and Directory backup configurations.
- Database connection strings follow the correct template for each provider.
- Cron expressions can be created without manual string entry.
- The guided tour correctly highlights these new interactive elements.
