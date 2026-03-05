# Track Specification: Enterprise Observability & Job History

## Overview
Enterprise users require deep visibility into automated tasks. This track implements a persistent history of every job execution, including verbose logs, status transitions, and duration metrics, accessible through a drill-down UI.

## Objectives
- **Execution Persistence:** Store every job run in a new `JobRuns` table.
- **Verbose Logging:** Capture detailed steps (e.g., "Starting hash check for X", "S3 Upload 45% complete") during the execution and associate them with the job run.
- **Drill-down UI:** Implement a "Jenkins-style" history view where users can click on a specific run to see its full console output and metrics.
- **Peace of Mind:** Provide immediate visual confirmation of what was processed, skipped, or failed.

## Key Components
- **`JobRuns` Table:** Stores `Id`, `JobId`, `StartTime`, `EndTime`, `Status`, and `Logs` (stored as a large text block or separate related table).
- **`Orchestrator` Logging Refactor:** Replace basic status updates with a verbose logging mechanism that stream data to both SignalR and the database.
- **Run History API:** New endpoints to fetch execution history for a specific job.
- **History Detail Component:** A frontend view for inspecting individual run logs.

## Success Criteria
- Every manual or scheduled job execution creates a persistent record in the database.
- Users can view a list of recent runs for any job.
- Clicking a run reveals a verbose, timestamped log of the entire pipeline.
- The UI provides "Jenkins-style" visibility (auto-scrolling logs for active runs).
