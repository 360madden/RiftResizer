# Project Scope

## Mission
RiftResizer is a Windows layout manager for multiple **RIFT** game clients.

Its purpose is simple:
- detect multiple RIFT windows
- place them predictably on one or more displays
- generate practical tiled layouts
- save and restore named layouts

## Intended Scale
- support up to **5 RIFT instances per display**
- support one or more displays
- support repeatable, profile-based placement

## What This Project Is
- a **window layout and profile tool**
- a **RIFT-specific desktop-side utility**
- a project centered on **tiling, geometry, and restore reliability**

## What This Project Is Not
- not a full ISBoxer clone
- not an input broadcasting suite
- not a game automation framework
- not a kitchen-sink multibox product

## Core Feature Areas
1. **Window Detection**
   - find candidate RIFT client windows
   - inspect title, size, position, state, and monitor placement

2. **Layout Generation**
   - generate common layouts for 1 to 5 windows per display
   - support gaps, padding, taskbar avoidance, and monitor bounds

3. **Profile Management**
   - save named layouts
   - restore them later
   - validate monitor compatibility before applying

4. **Apply Engine**
   - move/resize windows deterministically
   - provide dry-run and apply modes
   - report failures clearly

## Initial Deliverable
The first real deliverable should be a tool that:
- detects RIFT windows
- displays current geometry
- applies a generated tiled layout
- saves and reloads a layout profile

## Long-Term Direction
Later versions may add:
- a visual layout editor
- drag-adjustment and snap-to-grid
- main-window promotion / swap-to-main concepts
- hotkeys for profile apply
- launch-time auto-capture of RIFT windows

These are later features, not day-one requirements.
