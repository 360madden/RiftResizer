using System.Text.Json;
using RiftResizer.Core;

namespace RiftResizer.Profiles;

// Version: 0.1.0
// Total Characters: 2180
// Purpose: Persist and reload layout profiles as JSON files under the user's AppData folder.

public sealed record SavedLayoutProfile(
    string ProfileName,
    string DisplayId,
    string DisplayName,
    DateTimeOffset SavedAtUtc,
    IReadOnlyList<WindowSlot> Slots);

public sealed class JsonProfileStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public string BaseDirectory { get; }

    public JsonProfileStore(string? baseDirectory = null)
    {
        BaseDirectory = baseDirectory
            ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RiftResizer",
                "profiles");

        Directory.CreateDirectory(BaseDirectory);
    }

    public string GetProfilePath(string profileName)
    {
        var safeName = string.Concat(profileName.Where(ch => !Path.GetInvalidFileNameChars().Contains(ch)));
        return Path.Combine(BaseDirectory, $"{safeName}.json");
    }

    public void Save(SavedLayoutProfile profile)
    {
        var path = GetProfilePath(profile.ProfileName);
        var json = JsonSerializer.Serialize(profile, JsonOptions);
        File.WriteAllText(path, json);
    }

    public SavedLayoutProfile Load(string profileName)
    {
        var path = GetProfilePath(profileName);
        var json = File.ReadAllText(path);
        var profile = JsonSerializer.Deserialize<SavedLayoutProfile>(json, JsonOptions);

        return profile ?? throw new InvalidOperationException($"Failed to deserialize profile '{profileName}'.");
    }

    public IReadOnlyList<string> ListProfiles()
    {
        return Directory
            .EnumerateFiles(BaseDirectory, "*.json", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

// End of file.
