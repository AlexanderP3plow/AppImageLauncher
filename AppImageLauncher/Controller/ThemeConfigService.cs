using System;
using System.IO;
using System.Text.Json;
using AppImageLauncher.Classes;
using Avalonia.Styling;

namespace AppImageLauncher.Controller;

public static class ThemeConfigService
{
    private static readonly string ConfigPath = Paths.ConfigJson;

    public static ThemeVariant LoadTheme()
    {
        if (!File.Exists(ConfigPath))
            return ThemeVariant.Default;

        try
        {
            var json = File.ReadAllText(ConfigPath);
            var doc = JsonDocument.Parse(json);
            var themeValue = doc.RootElement.GetProperty("Theme").GetString();
            
            return themeValue switch
            {
                "Default" =>  ThemeVariant.Default,
                "Light" => ThemeVariant.Light,
                _ => ThemeVariant.Default
            };
        }
        catch
        {
            return ThemeVariant.Default;
        }
    }
    public static void SaveTheme(ThemeVariant theme)
    {
        var json = File.ReadAllText(ConfigPath);
        var config = JsonSerializer.Deserialize<ApplicationConfiguration>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (config == null)
        {
            config = new ApplicationConfiguration();
        }
        config.Theme = theme.Key;
        var updatedJson = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(ConfigPath, updatedJson);
    }
}