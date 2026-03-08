# Memlane v1.1.0

Memlane is a simple folder backup and synchronization tool. It helps you keep your important files safe by managing automated backups through an easy-to-use dashboard.

## New in v1.1.0
- **Zstandard Compression:** High-ratio, pure C# compression for faster and smaller backups.
- **Improved Reporting:** Independent run numbers and more accurate dashboard metrics.
- **Enhanced UI:** High-contrast side pane with improved visibility and interactive tutorials.
- **Database Backups:** Restored support for SQL Server and MariaDB (11.6+).

## Key Features
- **Complete Backups:** Automatically detects if any file in your folder has changed. If a change is found, it performs a full backup of the entire folder.
- **Status at a Glance:** Jenkins-style weather icons and descriptive status text show you the health of your backup history.
- **Automatic Scheduling:** Full Cron support for automated background backups.
- **Ignore Patterns:** Support for `.memignore` style patterns to exclude temporary files.
- **Storage Management:** Automated retention policies to prune old backups.

## Deployment & Installation

### Windows Installer
1. Run `deployment/publish.ps1` to bundle the app.
2. Compile `deployment/Memlane.iss` with **Inno Setup**.
3. Run the generated `.exe` to install as a desktop app.

### Windows Service (Background)
To run Memlane as a persistent background service:
1. Open PowerShell as **Administrator**.
2. Run `.\deployment\install-service.ps1`.
3. The app will now run in the background and start automatically with Windows.
4. To remove, run `.\deployment\uninstall-service.ps1`.

### Docker
1. Ensure Docker and Docker Compose are installed.
2. Run `docker-compose up -d`.
3. Access the dashboard at `http://localhost:5237`.
4. Your data and backups will be persisted in the `./data` and `./backups` folders.

---
## License & Terms of Use

**IMPORTANT: READ CAREFULLY**

Memlane is provided **"AS IS"**, without warranty of any kind, express or implied. By using this software, you agree that:

1. **NO LIABILITY FOR DATA LOSS:** The developers shall not be liable for any data loss, corruption, or hardware damage resulting from the use of this software. It is your responsibility to verify your backups.
2. **USE AT YOUR OWN RISK:** This is a powerful tool. Improper configuration (such as choosing the wrong folders) can result in files being deleted or overwritten.
3. **NO GUARANTEE OF SUCCESS:** While the software checks for changes to ensure files are backed up, external factors (like disk errors or computer issues) may still cause a backup to fail.

*By installing Memlane, you acknowledge these risks and agree to protect the developers from any legal claims or damages.*
