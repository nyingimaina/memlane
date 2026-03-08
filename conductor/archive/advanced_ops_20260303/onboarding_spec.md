# Track Specification: Onboarding & User Tutorials (Joyride)

## Overview
This track introduces a guided tour system to Memlane using `react-joyride` to enhance user onboarding and reduce UI confusion. The system will automatically trigger the first time a user encounters a form or screen and will be accessible via a standard tutorial icon for manual activation.

## Objectives
- **Guided Tours:** Implement interactive, step-by-step guides for the `Dashboard`, `Jobs` list, and `JobForm`.
- **First-Time Activation:** Use `localStorage` to automatically launch the tour the first time a specific route is visited.
- **Manual Launch:** Add a standard `QuestionMarkCircleIcon` (Tutorial Icon) to headers/forms for on-demand tours.
- **DRY Architecture:** Create a centralized `TutorialRegistry` and a reusable `GuidedTour` component.

## Key Components
- **`react-joyride` Library:** For the guided tour engine.
- **`TutorialRegistry.ts`:** Centralized storage for all tour steps across the application.
- **`GuidedTour` Component:** A high-level component to manage tour state and state persistence.
- **`TutorialIcon` Component:** A standard UI element to trigger the tour manually.

## Success Criteria
- Tours trigger automatically on the first visit to key screens (Dashboard, JobForm).
- Users can manually restart the tour using the tutorial icon.
- Tours correctly target UI elements (even within Zest components) via stable CSS selectors.
- The system is extensible, allowing for new tours to be added with minimal code changes.
- Tour progress is persisted in `localStorage`.

## User Experience (UX)
- **Non-Intrusive:** Tours should be easy to skip.
- **Consistent Branding:** The tour bubbles and indicators should align with Memlane's visual aesthetic.
- **Contextual:** Steps should provide helpful, technical context as per product guidelines.
