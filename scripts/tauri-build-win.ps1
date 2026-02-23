param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "Building frontend bundle..."
npm --prefix Launcher.Tauri run build

Write-Host "Building bridge project..."
dotnet build Launcher.Bridge/Launcher.Bridge.csproj -c $Configuration

Write-Host "Building Tauri Rust shell..."
if ($Configuration -eq "Release") {
    cargo build --release --manifest-path Launcher.Tauri/src-tauri/Cargo.toml
} else {
    cargo build --manifest-path Launcher.Tauri/src-tauri/Cargo.toml
}
