using System.Text.Json;

namespace SimpleImageResizer.Properties;

/// <summary>
/// User-scoped application settings, persisted as JSON in %LocalAppData%\SimpleImageResizer\settings.json.
/// Drop-in replacement for the legacy ApplicationSettingsBase / Settings.Designer.cs pattern.
/// </summary>
public sealed class AppSettings
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SimpleImageResizer",
        "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static AppSettings? _default;

    /// <summary>Gets the singleton settings instance, loaded from disk on first access.</summary>
    public static AppSettings Default => _default ??= Load();

    // ── Settings ────────────────────────────────────────────────────────────
    public string DestinationDirectory { get; set; } = string.Empty;
    public int SimpleResizeSetting { get; set; } = 50;
    public bool OptionAddNumericSuffix { get; set; } = false;
    public bool OptionShowMessageBox { get; set; } = true;
    public bool OptionClearImages { get; set; } = true;
    public bool OptionUseAllProcessors { get; set; } = true;
    public int OptionJpgQuality { get; set; } = 70;
    public string CurrentCulture { get; set; } = "en-US";
    public string CurrentUICulture { get; set; } = "en-US";

    // ── Persistence ──────────────────────────────────────────────────────────

    private static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                string json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch
        {
            // Return defaults if the file is missing or corrupt.
        }

        return new AppSettings();
    }

    /// <summary>Persists the current settings to disk.</summary>
    public void Save()
    {
        try
        {
            string dir = Path.GetDirectoryName(SettingsPath)!;
            Directory.CreateDirectory(dir);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, JsonOptions));
        }
        catch
        {
            // Swallow save errors — settings are non-critical.
        }
    }
}
