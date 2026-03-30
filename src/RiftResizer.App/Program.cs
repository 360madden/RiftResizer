using RiftResizer.Core;
using RiftResizer.Layouts;
using RiftResizer.Profiles;
using RiftResizer.Windowing;

namespace RiftResizer.App;

// Version: 0.1.0
// Total Characters: 6599
// Purpose: CLI entry point for scanning RIFT windows, generating tiled layouts, saving profiles, and applying saved profiles.

internal static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return 0;
            }

            return args[0].ToLowerInvariant() switch
            {
                "scan" => RunScan(),
                "displays" => RunDisplays(),
                "generate" => RunGenerate(args),
                "save-profile" => RunSaveProfile(args),
                "apply-profile" => RunApplyProfile(args),
                "list-profiles" => RunListProfiles(),
                _ => Fail($"Unknown command: {args[0]}")
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static int RunScan()
    {
        var windows = WindowScanner.GetRiftWindows();

        if (windows.Count == 0)
        {
            Console.WriteLine("No RIFT windows detected.");
            return 0;
        }

        foreach (var window in windows)
        {
            Console.WriteLine($"{window.Handle}: {window.ProcessName} | {window.Title} | {FormatRect(window.Bounds)}");
        }

        return 0;
    }

    private static int RunDisplays()
    {
        foreach (var display in DisplayScanner.GetDisplays())
        {
            Console.WriteLine($"{display.Id} | {display.Name} | Primary={display.IsPrimary} | Work={FormatRect(display.WorkingArea)}");
        }

        return 0;
    }

    private static int RunGenerate(string[] args)
    {
        var count = ParseCount(args, 1);
        var display = ResolveDisplay(args, 2);
        var template = LayoutGenerator.Generate(display, count);

        PrintTemplate(template, display);
        return 0;
    }

    private static int RunSaveProfile(string[] args)
    {
        if (args.Length < 3)
        {
            return Fail("Usage: save-profile <profile-name> <window-count> [display-index]");
        }

        var profileName = args[1];
        var count = ParseInt(args[2], "window-count");
        if (count < 1 || count > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "window-count must be between 1 and 5.");
        }

        var display = ResolveDisplay(args, 3);
        var template = LayoutGenerator.Generate(display, count);

        var store = new JsonProfileStore();
        store.Save(new SavedLayoutProfile(
            ProfileName: profileName,
            DisplayId: display.Id,
            DisplayName: display.Name,
            SavedAtUtc: DateTimeOffset.UtcNow,
            Slots: template.Slots));

        Console.WriteLine($"Saved profile '{profileName}' to {store.GetProfilePath(profileName)}");
        return 0;
    }

    private static int RunApplyProfile(string[] args)
    {
        if (args.Length < 2)
        {
            return Fail("Usage: apply-profile <profile-name>");
        }

        var store = new JsonProfileStore();
        var profile = store.Load(args[1]);

        var windows = WindowScanner.GetRiftWindows();
        if (windows.Count == 0)
        {
            return Fail("No RIFT windows detected.");
        }

        var orderedWindows = windows.Take(profile.Slots.Count).ToList();
        var results = WindowPlacer.Apply(orderedWindows, profile.Slots);

        foreach (var result in results)
        {
            Console.WriteLine($"{(result.Success ? "OK" : "FAIL")} | {result.Message}");
        }

        return results.All(result => result.Success) ? 0 : 1;
    }

    private static int RunListProfiles()
    {
        var store = new JsonProfileStore();
        foreach (var name in store.ListProfiles())
        {
            Console.WriteLine(name);
        }

        return 0;
    }

    private static DisplayInfo ResolveDisplay(string[] args, int indexPosition)
    {
        var displays = DisplayScanner.GetDisplays();
        if (displays.Count == 0)
        {
            throw new InvalidOperationException("No displays detected.");
        }

        if (args.Length <= indexPosition)
        {
            return displays.First(display => display.IsPrimary);
        }

        var index = ParseInt(args[indexPosition], "display-index");
        if (index < 0 || index >= displays.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"Display index must be between 0 and {displays.Count - 1}.");
        }

        return displays[index];
    }

    private static int ParseCount(string[] args, int index)
    {
        if (args.Length <= index)
        {
            throw new InvalidOperationException("Usage: generate <window-count> [display-index]");
        }

        var count = ParseInt(args[index], "window-count");
        if (count < 1 || count > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "window-count must be between 1 and 5.");
        }

        return count;
    }

    private static int ParseInt(string value, string name)
    {
        return int.TryParse(value, out var parsed)
            ? parsed
            : throw new InvalidOperationException($"Invalid {name}: '{value}'.");
    }

    private static void PrintTemplate(LayoutTemplate template, DisplayInfo display)
    {
        Console.WriteLine($"Display: {display.Name}");
        Console.WriteLine($"Layout: {template.Kind}");

        foreach (var slot in template.Slots)
        {
            Console.WriteLine($"Slot {slot.SlotIndex}: {FormatRect(slot.TargetRect)} [{slot.Label}]");
        }
    }

    private static string FormatRect(WindowRect rect)
    {
        return $"X={rect.X}, Y={rect.Y}, W={rect.Width}, H={rect.Height}";
    }

    private static int Fail(string message)
    {
        Console.Error.WriteLine(message);
        return 1;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("RiftResizer commands:");
        Console.WriteLine("  displays");
        Console.WriteLine("  scan");
        Console.WriteLine("  generate <window-count> [display-index]");
        Console.WriteLine("  save-profile <profile-name> <window-count> [display-index]");
        Console.WriteLine("  apply-profile <profile-name>");
        Console.WriteLine("  list-profiles");
    }
}

// End of file.
