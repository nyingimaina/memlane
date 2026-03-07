# Implementation Plan: v1.0.0 Stabilization

## Phase 1: UI Simplification
### [x] Task: Hide Database Backup Options
- [x] Implement: Update `JobForm.tsx` to remove the Database/Directory toggle.
- [x] Implement: Remove the `jobCategory === 'Database'` conditional sections from `JobForm.tsx`.
- [x] Implement: Ensure `initialConfig` defaults to Directory-safe values.
- [x] Refactor: Migrate `JobForm` to a Vertical Slice Architecture (`features/JobManagement`).
- [x] Style: Implement CSS modules and `theme.module.css` for consistent styling.

## Phase 2: Windows Installer
### [x] Task: Installer Strategy & Research
- [x] Implement: Research and compare Inno Setup vs. Velopack (Selected Inno Setup).
- [x] Implement: Create a `deployment/` directory with a draft installer script (`Memlane.iss`).
- [x] Implement: Create `publish.ps1` to automate Frontend build and .NET publishing.

## Phase 3: Final 1.0.0 Polish
### [ ] Task: README & Documentation
- [ ] Implement: Update `README.md` to reflect v1.0.0 focus.
- [ ] Implement: Final build and lint check.
