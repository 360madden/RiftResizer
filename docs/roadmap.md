# Roadmap

## Phase 0 — Repository Setup
- initialize repository
- create baseline documentation
- define module boundaries

## Phase 1 — Discovery
- confirm target platform assumptions
- identify RIFT window titles/classes/process behavior
- define exact resize/layout requirements
- decide whether the first deliverable is CLI-only or includes UI

## Phase 2 — Minimal Working Tool
- detect candidate RIFT windows
- report current window geometry
- apply manual resize and move operations
- add basic error handling and logging

## Phase 3 — Profiles
- save named layouts
- restore layouts reliably
- detect missing or changed monitor setups

## Phase 4 — Quality / Hardening
- input validation
- safer failure behavior
- structured logs
- packaging and release process

## Non-Goals for Initial Version
- deep automation beyond window management
- fragile heuristics that depend on timing guesses
- broad feature sprawl before the core resize workflow is solid
