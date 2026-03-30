using RiftResizer.Core;

namespace RiftResizer.Layouts;

// Version: 0.1.0
// Total Characters: 7032
// Purpose: Generate deterministic tiled layouts for one to five RIFT windows on a single display.

public static class LayoutGenerator
{
    public static LayoutTemplate Generate(
        DisplayInfo display,
        int windowCount,
        int outerMargin = 6,
        int gap = 6,
        LayoutTemplateKind preferredKind = LayoutTemplateKind.Auto)
    {
        if (windowCount < 1 || windowCount > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(windowCount), "RiftResizer currently supports 1 to 5 windows per display.");
        }

        var kind = preferredKind == LayoutTemplateKind.Auto
            ? ResolveAutoKind(display, windowCount)
            : preferredKind;

        return kind switch
        {
            LayoutTemplateKind.SoloFull => new LayoutTemplate(kind, display.Id, BuildSolo(display, outerMargin)),
            LayoutTemplateKind.DualVertical => new LayoutTemplate(kind, display.Id, BuildDualVertical(display, outerMargin, gap)),
            LayoutTemplateKind.DualHorizontal => new LayoutTemplate(kind, display.Id, BuildDualHorizontal(display, outerMargin, gap)),
            LayoutTemplateKind.TripleMainLeft => new LayoutTemplate(kind, display.Id, BuildTripleMainLeft(display, outerMargin, gap)),
            LayoutTemplateKind.QuadGrid => new LayoutTemplate(kind, display.Id, BuildQuadGrid(display, outerMargin, gap)),
            LayoutTemplateKind.FiveTopTwoBottomThree => new LayoutTemplate(kind, display.Id, BuildFive(display, outerMargin, gap)),
            _ => throw new NotSupportedException($"Unsupported layout kind: {kind}")
        };
    }

    private static LayoutTemplateKind ResolveAutoKind(DisplayInfo display, int windowCount)
    {
        var wide = display.WorkingArea.Width >= display.WorkingArea.Height;

        return windowCount switch
        {
            1 => LayoutTemplateKind.SoloFull,
            2 => wide ? LayoutTemplateKind.DualVertical : LayoutTemplateKind.DualHorizontal,
            3 => LayoutTemplateKind.TripleMainLeft,
            4 => LayoutTemplateKind.QuadGrid,
            5 => LayoutTemplateKind.FiveTopTwoBottomThree,
            _ => LayoutTemplateKind.Auto
        };
    }

    private static IReadOnlyList<WindowSlot> BuildSolo(DisplayInfo display, int margin)
    {
        var area = Shrink(display.WorkingArea, margin);
        return
        [
            new WindowSlot(1, display.Id, area, "solo")
        ];
    }

    private static IReadOnlyList<WindowSlot> BuildDualVertical(DisplayInfo display, int margin, int gap)
    {
        var area = Shrink(display.WorkingArea, margin);
        var width = (area.Width - gap) / 2;
        var left = new WindowRect(area.X, area.Y, width, area.Height);
        var right = new WindowRect(area.X + width + gap, area.Y, area.Width - width - gap, area.Height);

        return
        [
            new WindowSlot(1, display.Id, left, "left"),
            new WindowSlot(2, display.Id, right, "right")
        ];
    }

    private static IReadOnlyList<WindowSlot> BuildDualHorizontal(DisplayInfo display, int margin, int gap)
    {
        var area = Shrink(display.WorkingArea, margin);
        var height = (area.Height - gap) / 2;
        var top = new WindowRect(area.X, area.Y, area.Width, height);
        var bottom = new WindowRect(area.X, area.Y + height + gap, area.Width, area.Height - height - gap);

        return
        [
            new WindowSlot(1, display.Id, top, "top"),
            new WindowSlot(2, display.Id, bottom, "bottom")
        ];
    }

    private static IReadOnlyList<WindowSlot> BuildTripleMainLeft(DisplayInfo display, int margin, int gap)
    {
        var area = Shrink(display.WorkingArea, margin);
        var leftWidth = (int)Math.Round((area.Width - gap) * 0.62);
        var rightWidth = area.Width - leftWidth - gap;
        var rightHeightTop = (area.Height - gap) / 2;

        var left = new WindowRect(area.X, area.Y, leftWidth, area.Height);
        var rightTop = new WindowRect(area.X + leftWidth + gap, area.Y, rightWidth, rightHeightTop);
        var rightBottom = new WindowRect(area.X + leftWidth + gap, area.Y + rightHeightTop + gap, rightWidth, area.Height - rightHeightTop - gap);

        return
        [
            new WindowSlot(1, display.Id, left, "main"),
            new WindowSlot(2, display.Id, rightTop, "side-top"),
            new WindowSlot(3, display.Id, rightBottom, "side-bottom")
        ];
    }

    private static IReadOnlyList<WindowSlot> BuildQuadGrid(DisplayInfo display, int margin, int gap)
    {
        var area = Shrink(display.WorkingArea, margin);
        var cellWidth = (area.Width - gap) / 2;
        var cellHeight = (area.Height - gap) / 2;

        return
        [
            new WindowSlot(1, display.Id, new WindowRect(area.X, area.Y, cellWidth, cellHeight), "top-left"),
            new WindowSlot(2, display.Id, new WindowRect(area.X + cellWidth + gap, area.Y, area.Width - cellWidth - gap, cellHeight), "top-right"),
            new WindowSlot(3, display.Id, new WindowRect(area.X, area.Y + cellHeight + gap, cellWidth, area.Height - cellHeight - gap), "bottom-left"),
            new WindowSlot(4, display.Id, new WindowRect(area.X + cellWidth + gap, area.Y + cellHeight + gap, area.Width - cellWidth - gap, area.Height - cellHeight - gap), "bottom-right")
        ];
    }

    private static IReadOnlyList<WindowSlot> BuildFive(DisplayInfo display, int margin, int gap)
    {
        var area = Shrink(display.WorkingArea, margin);

        var topHeight = (area.Height - gap) / 2;
        var bottomHeight = area.Height - topHeight - gap;

        var topLeftWidth = (area.Width - gap) / 2;
        var topRightWidth = area.Width - topLeftWidth - gap;

        var bottomColWidth = (area.Width - (gap * 2)) / 3;
        var bottomLastWidth = area.Width - bottomColWidth - bottomColWidth - (gap * 2);

        return
        [
            new WindowSlot(1, display.Id, new WindowRect(area.X, area.Y, topLeftWidth, topHeight), "top-left"),
            new WindowSlot(2, display.Id, new WindowRect(area.X + topLeftWidth + gap, area.Y, topRightWidth, topHeight), "top-right"),
            new WindowSlot(3, display.Id, new WindowRect(area.X, area.Y + topHeight + gap, bottomColWidth, bottomHeight), "bottom-left"),
            new WindowSlot(4, display.Id, new WindowRect(area.X + bottomColWidth + gap, area.Y + topHeight + gap, bottomColWidth, bottomHeight), "bottom-center"),
            new WindowSlot(5, display.Id, new WindowRect(area.X + (bottomColWidth * 2) + (gap * 2), area.Y + topHeight + gap, bottomLastWidth, bottomHeight), "bottom-right")
        ];
    }

    private static WindowRect Shrink(WindowRect rect, int margin)
    {
        return new WindowRect(
            rect.X + margin,
            rect.Y + margin,
            Math.Max(0, rect.Width - (margin * 2)),
            Math.Max(0, rect.Height - (margin * 2)));
    }
}

// End of file.
