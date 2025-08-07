using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AppImageLauncher.Classes;
using AppImageLauncher.Controller;
using AppImageLauncher.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AppImageLauncher.Views;

public partial class NewApp : Window
{
    private readonly MainWindowViewModel _mainVm;
    public NewApp(MainWindowViewModel mainVm)
    {
        DataContext = new NewAppViewModel();
        (DataContext as NewAppViewModel)?.ReadConfigFile();
        InitializeComponent();
        _mainVm = mainVm;
    }

    [Obsolete("Obsolete")]
    private async void OpenImageFileDialog(object? sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Title = "Datei öffnen",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Images", Extensions = { "png", "jpg", "jpeg"} },
                new FileDialogFilter { Name = "All Files Dateien", Extensions = { "*" } }
            }
        };
        var result = await dialog.ShowAsync(this);
        if (result is { Length: > 0 })
        {
            IconFile.Text = result![0];
        }
    }
    [Obsolete("Obsolete")]
    private async void OpenExecutableFileDialog(object? sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Title = "Datei öffnen",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "AppImages", Extensions = { "AppImage"} },
                new FileDialogFilter { Name = "Alle Dateien", Extensions = { "*" } }
            }
        };
        var result = await dialog.ShowAsync(this);
        if (result is { Length: > 0 })
        {
            Executable.Text = result![0];
        }
    }
    private async void WriteNewApp(object? sender, RoutedEventArgs e)
    {
        var app = new Application
        {
            Name = NewAppName.Text,
            IconPath = IconFile.Text,
            AppImagePath = Executable.Text,
            Argument = NewAppArg.Text,
            Categories = NewAppViewModel.SelectedCategories,
            RunTime = TimeSpan.Parse("00:00")
        };
        await Cache.WriteCache(app);
        await _mainVm.ReadCache();
        Close();
    }
    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }
    private void SetSelectedCategories(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is ListBox categoryOverview)
        {
            if (categoryOverview.SelectedItems == null) return;
            var selectedCategories = categoryOverview.SelectedItems
                .OfType<object>() 
                .Select(item => item?.ToString())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            (DataContext as NewAppViewModel)?.SetSelectedCategories(selectedCategories);
        }
    }
}