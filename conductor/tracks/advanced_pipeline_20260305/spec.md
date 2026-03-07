# Track Specification: Advanced Pipeline & 7z Compression

## Overview
Enhance the backup pipeline to support professional-grade 7z compression and a refined "Pick -> Compress -> Move" workflow. This transition from standard ZIP to 7z will provide significantly better compression ratios and more robust multi-part archive capabilities.

## Requirements
- **7z Integration:** Integrate `7z.exe` (via command line) to handle archive creation.
- **Improved Pipeline:** Implement a strict sequence:
    1. **Pick:** Identify changed files (Mirror to workspace).
    2. **Compress:** Create a 7z archive of the workspace.
    3. **Move:** Atomically move the archive to the final destination.
- **UI Updates:** Add 7z-specific options (Compression level, password protection) to the Job Management UI.
- **Cleanup:** Ensure temporary compression artifacts are purged after a successful move.

## Technical Strategy
- Bundle `7z.exe` or detect its presence in the system.
- Use `System.Diagnostics.Process` to execute compression tasks.
- Maintain the "All-or-Nothing" mirror logic for the "Pick" phase.
