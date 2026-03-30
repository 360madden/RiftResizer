# Windowing Module

Responsibilities:
- enumerate candidate RIFT windows
- inspect current geometry and state
- resolve display/work-area association
- apply move and resize operations
- expose safe, deterministic window-management primitives

This module should remain Windows-specific unless there is a real need to abstract it further.
