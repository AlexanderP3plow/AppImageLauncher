using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AppImageLauncher.Classes;
using AppImageLauncher.Controller;
using Avalonia.Styling;

namespace AppImageLauncher.ViewModels;

public class MainWindowViewModel : ViewModelBase,  INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<Application> ApplicationOverview { get; set; } = new();
    public async Task ReadCache()
    {
        var cache = await Cache.ReadCache();
        ApplicationOverview.Clear();
        foreach (var app in cache)
        {
            ApplicationOverview.Add(app);
        }
    }

    public static ApplicationConfiguration ConfigFileContent { get; set; } = new();
    public void ReadConfigFile()
    {
        var configFileContent = Cache.ReadConfig();
        configFileContent.Categories!.Insert(0, "All");
        ConfigFileContent =  configFileContent;
    }
    private bool? _toggleButtonChecked;
    private string? _selectedTheme;

    public string? SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            _selectedTheme = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTheme)));
        }
    }
    public bool? ToggleButtonChecked
    {
        get => _toggleButtonChecked;
        set
        {
            if (_toggleButtonChecked != value)
            {
                _toggleButtonChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ToggleButtonChecked)));

                switch (value)
                {
                    case true:
                        ThemeManager.CurrentTheme = ThemeVariant.Default;
                        SelectedTheme = "Default";
                        break;
                    case false:
                        ThemeManager.CurrentTheme = ThemeVariant.Light;
                        SelectedTheme = "Light";
                        break;
                }
                ThemeConfigService.SaveTheme(ThemeManager.CurrentTheme); 
            }
        }
    }
    public async void SetSelectedCategories(List<string?> selectedCategoryItems)
    {
        var cache = await Cache.ReadCache();
        ApplicationOverview.Clear();

        if (selectedCategoryItems.Any(s => s != null && s.Contains("All")))
        {
            foreach (var app in cache)
            {
                ApplicationOverview.Add(app);
            }
        }
        else
        {
            foreach (var app in cache)
            {
                if (app.Categories != null && selectedCategoryItems.Any(cat => app.Categories.Contains(cat)))
                {
                    ApplicationOverview.Add(app);
                }
            }
        }
    }
}