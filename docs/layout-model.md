# Layout Model

## Goal
Represent window placement in a way that is easy to generate, validate, save, and re-apply.

## Core Concepts

### Display
Represents a physical monitor or usable work area.

Fields:
- display id
- display name
- full bounds
- working bounds
- is primary
- dpi / scaling metadata if needed

### WindowRect
Represents a target rectangle.

Fields:
- x
- y
- width
- height

### WindowSlot
Represents one intended client position inside a layout.

Fields:
- slot index
- display id
- target rect
- z-order hint (optional)
- label (optional)

### LayoutTemplate
A generated arrangement before binding to actual windows.

Fields:
- template name
- display count assumptions
- slots
- generator settings

### LayoutProfile
A saved layout intended for reuse.

Fields:
- profile name
- layout template origin
- assigned displays
- saved slots
- optional notes
- created / updated timestamps

## Generator Requirements
The generator should support:
- 1 window full-screen-in-work-area
- 2 window vertical split
- 2 window horizontal split
- 3 window tiled arrangements
- 4 window grid
- 5 window tiled arrangements

## Geometry Rules
- stay inside working area unless user explicitly wants full display bounds
- support configurable outer margin
- support configurable inner gap
- avoid overlap by default
- preserve deterministic ordering of slots

## First Templates Worth Building
- `solo_full`
- `dual_vertical`
- `dual_horizontal`
- `triple_main_left`
- `triple_main_top`
- `quad_grid`
- `five_grid`
- `five_main_plus_grid`

## Multi-Display Handling
A profile should be able to define:
- how many windows go to each display
- which display is preferred for the first or main slot
- whether a profile is strict to one monitor setup or can remap loosely

## Apply Strategy
Applying a profile should:
1. detect candidate RIFT windows
2. sort them deterministically
3. assign windows to slots
4. validate slot/display availability
5. move and resize windows
6. report which moves succeeded or failed
