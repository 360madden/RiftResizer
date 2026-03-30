# Architecture

## Current Intent
RiftResizer is being initialized as a focused repository for RIFT MMO window resizing, layout control, and related desktop-side tooling.

## Design Principles
- Keep modules small and clearly separated.
- Avoid coupling UI logic, window-management logic, and any game-specific detection logic.
- Prefer deterministic behavior over clever automation.
- Keep room for later support for profiles and multi-window layouts.

## Proposed Module Areas
1. **Core**
   - shared models
   - configuration loading/saving
   - logging

2. **Window Management**
   - enumerate RIFT windows
   - inspect position/size/state
   - resize/reposition operations
   - optional monitor-aware placement

3. **Profiles**
   - save named layouts
   - restore known layouts
   - validate profile compatibility with detected windows/displays

4. **UI / CLI Layer**
   - a minimal launcher, CLI, or desktop UI
   - profile selection
   - dry-run vs apply mode

## Repo Layout
- `src/` — implementation code
- `docs/` — documentation and planning
- `assets/` — static assets and screenshots if needed

## Immediate Goal
Establish a clean base structure before implementation begins.
