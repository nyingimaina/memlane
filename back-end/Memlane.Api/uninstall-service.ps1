# Memlane Service Uninstallation Script
# Run as Administrator

$ServiceName = "MemlaneBackupService"

Write-Host "Stopping and removing $ServiceName..." -ForegroundColor Yellow

if (Get-Service $ServiceName -ErrorAction SilentlyContinue) {
    Stop-Service -Name $ServiceName -ErrorAction SilentlyContinue
    Remove-Service -Name $ServiceName
    Write-Host "Service removed successfully." -ForegroundColor Green
} else {
    Write-Warning "Service $ServiceName not found."
}
