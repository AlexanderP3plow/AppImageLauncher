using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AppImageLauncher.Classes;

namespace AppImageLauncher.Controller;
public static class Paths
{
    public static readonly string MainPath = Environment.CurrentDirectory;
    public static readonly string AppImagesPath = Path.Combine(MainPath, "AppImages");
    public static readonly string IconsPath = Path.Combine(MainPath, "Icons");
    public static readonly string Config = Path.Combine(MainPath, "Config");
    public static readonly string ConfigJson = Path.Combine(Config, "config.json");
    public static readonly List<string> EnvironmentPaths = [AppImagesPath, IconsPath, Config];
    public static readonly string DirectoryStructure = Path.Combine(Config, "directoryStructure.db");
}
public static class Setup
{
    public static async Task AppEnvironment()
    {
        try
        {
            foreach (var path in Paths.EnvironmentPaths)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }   
            }
            if (!File.Exists(Paths.ConfigJson))
            {
                await WriteConfigFile();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static async Task WriteConfigFile()
    {
        var config = new
        {
            Theme = "Default",
            Categories = new[]
            {
                "Application",
                "Development",
                "Audio",
                "Video",
                "Settings",
                "System"
            }
        };
        string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(Paths.ConfigJson, json);
    }
}