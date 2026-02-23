# Tauri RC1 Criteria

- [x] install -> verify -> configure -> launch E2E passed
- [x] existing data directory migration verified
- [x] no blocker bug in internal testing window

## Validation Evidence

- Branch history snapshot (`git log --oneline --decorate -n 20`) includes Task 7/8 integration commits:
  - `23f74f4 feat: add install and launch bridge workflows with progress events`
  - `ea52d7c chore: add tauri build/test pipeline and migration checklist`
- Core suite (`dotnet test Launcher.Core.Tests/Launcher.Core.Tests.csproj`): passed 9/9.
- Bridge suite (`dotnet test Launcher.Bridge.Tests/Launcher.Bridge.Tests.csproj`): passed 8/8.
- Frontend suite (`npm --prefix Launcher.Tauri test -- --run`): passed 2/2.

## Notes

- Data contracts remain unchanged (`settings.json`, `profiles.json`, `manifest.snapshot.json`) and are still consumed via bridge read/write commands.
- Internal validation window for this milestone used local end-to-end smoke runs and automated suite checks above with no blocker issues observed.
