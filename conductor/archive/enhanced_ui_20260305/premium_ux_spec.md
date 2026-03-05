# Track Specification: Premium UX Refinement (Pickers & Modular UI)

## Overview
This track elevates Memlane's UX by replacing manual path entry with interactive directory pickers and refactoring the `JobForm` into modular, DRY components.

## Objectives
- **Directory Pickers:** Implement a UI component that allows users to browse and select local directories (or common paths) instead of manual typing.
- **Modular Job Configuration:** Refactor the `JobForm` into smaller sub-components (e.g., `BackupSourceConfig`, `StorageDestinationConfig`, `ScheduleConfig`) to eliminate code duplication.
- **Input Minimization:** Review all form fields and prioritize interactive elements (dropdowns, toggles) over raw text input.
- **DRY Styling:** Ensure all new components strictly follow the `jattac.libs.web.zest-*` design system.

## Key Components
- **`DirectoryPicker` Component:** An interactive component for selecting file/folder paths.
- **`JobForm` Refactor:** Decompose `JobForm.tsx` into specialized sub-components.
- **`Zest` Integration:** Enhanced use of Zest components for a premium feel.

## Success Criteria
- Users can configure a full backup pipeline with minimal keyboard interaction.
- Directory and target paths are selected via a picker interface.
- `JobForm` code is clean, modular, and free of redundant logic.
- Guided tour accurately reflects the new interactive picker workflow.
