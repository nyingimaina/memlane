# Track Specification: Advanced Job Monitoring & Health

## Overview
Elevate the main dashboard visibility by adding Jenkins-inspired monitoring columns. This provides an at-a-glance understanding of job stability, upcoming schedules, and execution history.

## Objectives
- **Job Health (Weather Icons):** Calculate health based on the success/failure ratio of the last 5 runs.
    - ☀️ (Sunny): 5/5 or 4/5 successful.
    - ⛅ (Partly Cloudy): 3/5 successful.
    - ☁️ (Cloudy): 2/5 successful.
    - 🌧️ (Rainy): 1/5 successful.
    - ⛈️ (Stormy): 0/5 successful.
- **Run Numbering:** Display the "Last Run Number" (e.g., #14) in the table.
- **Next Run Visibility:** Clearly show the next scheduled execution time for cron-enabled jobs.
- **Enterprise UI:** Professional icons and precise data presentation.

## Key Components
- **Job Health Service:** Backend logic to aggregate recent run results and determine a stability score.
- **Model Updates:** Include `LastRunId` and `HealthScore` in the job overview payload.
- **JobsTable Refactor:** Add the new columns and integrated iconography.

## Success Criteria
- The Jobs table shows a "Health" column with intuitive weather icons.
- Users can see the exact number of the most recent execution.
- Scheduled jobs show their "Next Run" time in a dedicated column.
- The UI feels premium and information-dense as per project guidelines.
