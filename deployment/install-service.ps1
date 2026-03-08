# Memlane Service Installation Script
# Run as Administrator

$ServiceName = "MemlaneBackupService"
$ExePath = Join-Path $PSScriptRoot "Memlane.Api.exe"

if (!(Test-Path $ExePath)) {
    Write-Error "Memlane.Api.exe not found in $PSScriptRoot. Please run publish.ps1 first."
    exit
}

Write-Host "Installing $ServiceName..." -ForegroundColor Cyan

New-Service -Name $ServiceName `
            -BinaryPathName $ExePath `
            -DisplayName "Memlane Backup Service" `
            -Description "Automated background backup and synchronization service." `
            -StartupType Automatic

# Set to restart on failure
sc.exe failure $ServiceName reset= 86400 actions= restart/60000/restart/60000/restart/60000

Start-Service -Name $ServiceName

Write-Host "Service installed and started successfully." -ForegroundColor Green
