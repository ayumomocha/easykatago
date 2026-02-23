# Tauri Migration Checklist

Date: 2026-02-23

## Data Compatibility
- [ ] Existing `data/settings.json` can be read without conversion.
- [ ] Existing `data/profiles.json` can be read without conversion.
- [ ] Existing `data/manifest.snapshot.json` can be read without conversion.

## Core Workflow Parity
- [ ] Install command can be triggered from Tauri shell.
- [ ] Launch command can be triggered from Tauri shell.
- [ ] Settings read/write round-trip works through bridge.
- [ ] Profiles read/write round-trip works through bridge.

## Verification Commands
- [ ] `dotnet test Launcher.Core.Tests/Launcher.Core.Tests.csproj`
- [ ] `dotnet test Launcher.Bridge.Tests/Launcher.Bridge.Tests.csproj`
- [ ] `npm --prefix Launcher.Tauri run lint`
- [ ] `npm --prefix Launcher.Tauri test -- --run`
- [ ] `cargo test --manifest-path Launcher.Tauri/src-tauri/Cargo.toml`

## Packaging
- [ ] `scripts/tauri-build-win.ps1` runs successfully on Windows.
- [ ] CI workflow `.github/workflows/tauri-dev.yml` is green.
