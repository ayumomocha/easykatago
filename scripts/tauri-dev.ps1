param(
    [switch]$SkipInstall
)

$ErrorActionPreference = "Stop"

if (-not $SkipInstall) {
    npm --prefix Launcher.Tauri install
}

Write-Host "Starting Tauri frontend dev server..."
npm --prefix Launcher.Tauri run dev
