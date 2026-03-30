# External References

These references help define the design direction for RiftResizer.

## ISBoxer
- Window Layout concepts: regions, home regions, swap groups, active region, generator-driven layout creation
- Window Layout generator: monitor usage, aspect ratio handling, generated styles, swap-group based distribution
- GPU management: cross-monitor swapping can carry a significant performance penalty depending on hardware/display topology

References:
- https://isboxer.com/wiki/Window_Layout
- https://isboxer.com/wiki/Window_Layout_generator
- https://isboxer.com/wiki/GPU_Management

## OpenMultiBoxing
- auto layout setup
- drag-to-reposition
- drag-to-resize
- snap-to-grid
- 1-pixel nudge with arrow keys
- stay-on-top toggle within layout UI

Reference:
- https://openmultiboxing.org/help.html

## Intended Use of These References
RiftResizer should copy the practical layout-management ideas and avoid unnecessary product sprawl.
It should not attempt to duplicate every feature of these tools.
