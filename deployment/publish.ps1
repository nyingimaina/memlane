# Memlane Build & Publish Script for v1.0.0

$ErrorActionPreference = "Stop"

$root = Get-Item $PSScriptRoot\..
$frontendDir = Join-Path $root.FullName "front-end"
$backendDir = Join-Path $root.FullName "back-end\Memlane.Api"
$publishDir = Join-Path $backendDir "bin\Release\net10.0\publish"

Write-Host ">>> 1. Building Frontend (Next.js Static Export)..." -ForegroundColor Cyan
Set-Location $frontendDir
# Ensure next.config.mjs has output: 'export'
npm run build

Write-Host ">>> 2. Preparing Backend wwwroot..." -ForegroundColor Cyan
$wwwroot = Join-Path $backendDir "wwwroot"
if (Test-Path $wwwroot) { Remove-Item $wwwroot -Recurse -Force }
New-Item -ItemType Directory -Path $wwwroot
Copy-Item -Path (Join-Path $frontendDir "out\*") -Destination $wwwroot -Recurse -Force

Write-Host ">>> 3. Publishing Backend (.NET 10 Self-Contained)..." -ForegroundColor Cyan
Set-Location $backendDir
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true

Write-Host "`n>>> Done! Deployment files are in: $publishDir" -ForegroundColor Green
Write-Host ">>> You can now run Inno Setup on 'deployment\Memlane.iss'" -ForegroundColor Yellow
