using System;
using System.Collections.Generic;
using AppImageLauncher.Classes;
using AppImageLauncher.Controller;

namespace AppImageLauncher.ViewModels;

public class EditAppViewModel
{
    public ApplicationConfiguration ConfigFileContent { get; set; } = new();
    
    public void ReadConfigFile()
    {
        var configFileContent = Cache.ReadConfig();
        ConfigFileContent = configFileContent;
    }
    public static List<string>? SelectedCategories { get; set; } = new();

    public void SetSelectedCategories(List<string?> selectedCategories)
    {
        SelectedCategories?.Clear();
        foreach (var category in selectedCategories)
        {
            SelectedCategories?.Add(category!);
        }
    }
}