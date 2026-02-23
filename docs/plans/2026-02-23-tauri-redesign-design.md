# EasyKataGo Tauri Redesign (Windows First) - Design

Date: 2026-02-23
Status: Approved by user
Scope: Windows-first migration, `dev` branch only, hybrid architecture

## 1. Context

Current app is a WPF desktop client (`Launcher.App`) with large workflow logic for install/launch/network/profile management and a stable core library (`Launcher.Core`).

Main pain points:
- UI quality and consistency are not at target level.
- Future cross-platform delivery is difficult with WPF-only stack.
- Existing install/launch logic is valuable and should not be rewritten in one step.

## 2. Product Decisions (Locked)

- Migration strategy: **Windows first**.
- Technical route: **Hybrid migration**.
  - New UI shell: Tauri + Web frontend.
  - Keep current C# core/workflow capability in early phases.
  - Replace internals incrementally later if needed.
- Branch strategy: all redesign work in **`dev`** branch; keep `main` stable.

## 3. Goals and Non-goals

### Goals
- Deliver a modern, coherent desktop UI using Tauri frontend stack.
- Keep current core capability parity for the main user flow.
- Build a migration path to macOS/Linux without destabilizing current Windows delivery.
- Preserve existing user data compatibility (`data/` JSON files and runtime artifacts).

### Non-goals
- Full Rust rewrite in first phase.
- Immediate three-platform feature parity.
- Reworking business rules that already work in current C# flow.

## 4. Architecture

### Runtime Topology

1. Frontend (React + TypeScript recommended) renders all pages and interactions.
2. Tauri command layer (Rust) exposes a stable local API to frontend.
3. C# bridge worker handles current domain workflows.
4. Existing data files remain authoritative for settings/profiles/manifest snapshots.

### Integration Pattern

- Frontend never executes domain logic directly.
- All operations go through command contracts:
  - `install/*`
  - `launch/*`
  - `profiles/*`
  - `networks/*`
  - `logs/*`
  - `diagnostics/*`
  - `settings/*`
- Long-running jobs use event stream (progress/log/status updates).
- Errors are normalized to one user-facing error envelope.

## 5. UX and Visual Redesign Direction

### Information Architecture

Keep module semantics but modernize composition:
- Home
- Install
- Networks
- Profiles
- Logs
- Diagnostics
- Settings
- About

### Interaction Upgrades

- Home as an operations dashboard: status, quick actions, recent results.
- Install as a task board: queue, stages, retry, structured progress.
- Networks/Profiles as dual-pane workspace: list + detail/action panel.
- Logs and diagnostics as searchable, structured panels with copy/export actions.

### Design System

- Define design tokens for color, spacing, radius, typography, state styles.
- Build reusable component set before feature pages are fully migrated.
- Ensure responsive behavior for standard desktop and narrower window widths.

## 6. Branching and Delivery Strategy

- Long-lived integration branch: `dev`.
- Feature branches: `feature/<topic>` from `dev`.
- Merge policy:
  - `feature/* -> dev` only.
  - `main` receives changes from `dev` after release criteria are satisfied.
- `main` remains stable until Tauri version passes RC validation.

## 7. Milestones (Windows First)

- M0 Baseline setup (0.5-1 day)
  - Create `dev`, scaffold Tauri workspace, baseline CI.
- M1 Visual shell and navigation (2-4 days)
  - Core layout, nav, tokens, base components.
- M2 Bridge foundation (3-5 days)
  - settings/profiles/logs/health-check commands.
- M3 Core flow parity (5-8 days)
  - install/verify/launch and path auto-fix flow.
- M4 Stabilization and packaging (2-4 days)
  - error consistency, regression passes, Windows package outputs.

## 8. Risks and Mitigations

- Cross-process orchestration complexity
  - Mitigation: contract-first API and strict error envelope.
- Regression risk during workflow migration
  - Mitigation: reuse C# implementation initially; preserve existing tests.
- Scope creep to full rewrite
  - Mitigation: lock Windows-first and hybrid approach for current phase.

## 9. Test and Acceptance Criteria

- Build checks
  - Frontend build passes.
  - Tauri desktop build passes (Windows).
  - Existing `Launcher.Core.Tests` stays green.
- Functional checks
  - End-to-end path works: install -> verify -> configure -> launch.
  - Logs and diagnostics are viewable/exportable.
  - Existing `data/` from WPF version is readable without manual conversion.

## 10. Main-merge Gate

`dev -> main` requires all of:
- RC package tested internally with no blocker issues.
- Core user flow parity achieved and verified.
- CI gate stable for a sustained period.
- Rollback path documented and validated.

## 11. RC Tracking

- RC1 criteria and validation evidence are tracked in `docs/releases/tauri-rc1.md`.
- Implementation status follows the task plan in `docs/plans/2026-02-23-tauri-redesign-implementation.md`.
