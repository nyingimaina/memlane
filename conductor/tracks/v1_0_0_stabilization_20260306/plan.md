# Implementation Plan: v1.0.0 Stabilization

## Phase 1: UI Simplification
### [x] Task: Hide Database Backup Options
- [x] Implement: Update `JobForm.tsx` to remove the Database/Directory toggle.
- [x] Implement: Remove the `jobCategory === 'Database'` conditional sections from `JobForm.tsx`.
- [x] Implement: Ensure `initialConfig` defaults to Directory-safe values.

## Phase 2: Windows Installer
### [ ] Task: Installer Strategy & Research
- [ ] Implement: Research and compare Inno Setup vs. Velopack.
- [ ] Implement: Create a `deployment/` directory with a draft installer script (Inno Setup preferred for v1.0.0).

## Phase 3: Final 1.0.0 Polish
### [ ] Task: README & Documentation
- [ ] Implement: Update `README.md` to reflect v1.0.0 focus.
- [ ] Implement: Final build and lint check.
