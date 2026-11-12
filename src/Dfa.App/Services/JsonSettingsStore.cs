namespace Dfa.App.Services;

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

// AppSettings as a small JSON file, by default in the user's application data folder (local only).
public sealed class JsonSettingsStore : ISettingsStore
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly string _path;

    public JsonSettingsStore(string path) => _path = path;

    public static string DefaultPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RoPlate", "settings.json");

    public AppSettings Load()
    {
        try
        {
            return File.Exists(_path)
                ? JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_path), _options) ?? new AppSettings()
                : new AppSettings();
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or JsonException)
        {
            return new AppSettings();
        }
    }

    public void Save(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, JsonSerializer.Serialize(settings, _options));
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            // Settings are a convenience: if they cannot be saved, the app keeps working with the current values.
        }
    }
}
