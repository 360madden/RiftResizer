# Layouts Module

Responsibilities:
- generate tiled layouts for 1 to 5 windows per display
- calculate slot rectangles
- enforce margins, gaps, and work-area constraints
- expose deterministic template generation

This module should not know about live windows directly. It should focus on geometry generation only.
