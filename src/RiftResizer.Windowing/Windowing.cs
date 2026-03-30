using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using RiftResizer.Core;

namespace RiftResizer.Windowing;

// Version: 0.1.0
// Total Characters: 6224
// Purpose: Enumerate displays, find likely RIFT client windows, and apply move/resize operations through Win32.

public sealed record GameWindowInfo(
    nint Handle,
    string Title,
    string ProcessName,
    WindowRect Bounds);

public static class DisplayScanner
{
    public static IReadOnlyList<DisplayInfo> GetDisplays()
    {
        return Screen.AllScreens
            .Select((screen, index) => new DisplayInfo(
                Id: $"display-{index}",
                Name: screen.DeviceName,
                Bounds: new WindowRect(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height),
                WorkingArea: new WindowRect(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width, screen.WorkingArea.Height),
                IsPrimary: screen.Primary))
            .ToList();
    }
}

public static class WindowScanner
{
    public static IReadOnlyList<GameWindowInfo> GetRiftWindows()
    {
        var results = new List<GameWindowInfo>();

        NativeMethods.EnumWindows((handle, _) =>
        {
            if (!NativeMethods.IsWindowVisible(handle))
            {
                return true;
            }

            var title = GetWindowTitle(handle);
            if (string.IsNullOrWhiteSpace(title))
            {
                return true;
            }

            var processName = TryGetProcessName(handle);
            var looksLikeRift =
                processName.StartsWith("rift", StringComparison.OrdinalIgnoreCase) ||
                title.Contains("rift", StringComparison.OrdinalIgnoreCase);

            if (!looksLikeRift)
            {
                return true;
            }

            if (!NativeMethods.GetWindowRect(handle, out var rect))
            {
                return true;
            }

            var bounds = new WindowRect(
                rect.Left,
                rect.Top,
                rect.Right - rect.Left,
                rect.Bottom - rect.Top);

            results.Add(new GameWindowInfo(handle, title, processName, bounds));
            return true;
        }, nint.Zero);

        return results
            .OrderBy(window => window.Title, StringComparer.OrdinalIgnoreCase)
            .ThenBy(window => window.Handle)
            .ToList();
    }

    private static string GetWindowTitle(nint handle)
    {
        var length = NativeMethods.GetWindowTextLength(handle);
        if (length <= 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder(length + 1);
        _ = NativeMethods.GetWindowText(handle, builder, builder.Capacity);
        return builder.ToString();
    }

    private static string TryGetProcessName(nint handle)
    {
        _ = NativeMethods.GetWindowThreadProcessId(handle, out var processId);

        try
        {
            using var process = Process.GetProcessById((int)processId);
            return process.ProcessName;
        }
        catch
        {
            return string.Empty;
        }
    }
}

public static class WindowPlacer
{
    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpShowWindow = 0x0040;
    private const int SwRestore = 9;

    public static IReadOnlyList<ApplyResult> Apply(
        IReadOnlyList<GameWindowInfo> windows,
        IReadOnlyList<WindowSlot> slots)
    {
        var results = new List<ApplyResult>();

        var count = Math.Min(windows.Count, slots.Count);
        for (var i = 0; i < count; i++)
        {
            var window = windows[i];
            var slot = slots[i];

            _ = NativeMethods.ShowWindow(window.Handle, SwRestore);

            var ok = NativeMethods.SetWindowPos(
                window.Handle,
                nint.Zero,
                slot.TargetRect.X,
                slot.TargetRect.Y,
                slot.TargetRect.Width,
                slot.TargetRect.Height,
                SwpNoZOrder | SwpShowWindow);

            results.Add(ok
                ? new ApplyResult(true, $"Applied slot {slot.SlotIndex} to '{window.Title}'.")
                : new ApplyResult(false, $"Failed to apply slot {slot.SlotIndex} to '{window.Title}'."));
        }

        if (windows.Count != slots.Count)
        {
            results.Add(new ApplyResult(
                false,
                $"Window/slot count mismatch. Windows={windows.Count}, Slots={slots.Count}."));
        }

        return results;
    }
}

internal static partial class NativeMethods
{
    internal delegate bool EnumWindowsProc(nint hWnd, nint lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool EnumWindows(EnumWindowsProc lpEnumFunc, nint lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool IsWindowVisible(nint hWnd);

    [LibraryImport("user32.dll", EntryPoint = "GetWindowTextW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

    [LibraryImport("user32.dll")]
    internal static partial int GetWindowTextLength(nint hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetWindowRect(nint hWnd, out Rect lpRect);

    [LibraryImport("user32.dll")]
    internal static partial uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowPos(
        nint hWnd,
        nint hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        uint uFlags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(nint hWnd, int nCmdShow);
}

[StructLayout(LayoutKind.Sequential)]
internal struct Rect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}

// End of file.
