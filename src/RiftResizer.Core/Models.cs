namespace RiftResizer.Core;

// Version: 0.1.0
// Total Characters: 997
// Purpose: Shared models used by layout generation, profile persistence, and window application.

public readonly record struct WindowRect(int X, int Y, int Width, int Height)
{
    public int Right => X + Width;
    public int Bottom => Y + Height;
}

public sealed record DisplayInfo(
    string Id,
    string Name,
    WindowRect Bounds,
    WindowRect WorkingArea,
    bool IsPrimary);

public sealed record WindowSlot(
    int SlotIndex,
    string DisplayId,
    WindowRect TargetRect,
    string? Label = null);

public enum LayoutTemplateKind
{
    Auto = 0,
    SoloFull = 1,
    DualVertical = 2,
    DualHorizontal = 3,
    TripleMainLeft = 4,
    QuadGrid = 5,
    FiveTopTwoBottomThree = 6
}

public sealed record LayoutTemplate(
    LayoutTemplateKind Kind,
    string DisplayId,
    IReadOnlyList<WindowSlot> Slots);

public sealed record ApplyResult(
    bool Success,
    string Message);

// End of file.
