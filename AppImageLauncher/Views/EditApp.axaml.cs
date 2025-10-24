using System;
using System.Collections.Generic;
using System.Linq;
using AppImageLauncher.Controller;
using AppImageLauncher.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Application = AppImageLauncher.Classes.Application;

namespace AppImageLauncher.Views;

public partial class EditApp : Window
{
    private Application _application;
    private readonly MainWindowViewModel _mainVm;
    public EditApp(MainWindowViewModel viewModel, Application app)
    {
        _application = app;
        DataContext = new EditAppViewModel();
        InitializeComponent();
        _mainVm = viewModel;
        SetTextBoxText(app);
    }

    private void SetTextBoxText(Application app)
    {
        EditAppViewModel vm = new EditAppViewModel();
        vm.ReadConfigFile();
        AppName.Text = app.Name;
        Executable.Text = app.AppImagePath;
        IconFile.Text = app.IconPath;
        SelectedApp.ItemsSource = vm.ConfigFileContent.Categories;
        SelectedApp.Loaded += (_, _) =>
        {
            SelectedApp.SelectedItems?.Clear();
            foreach (var cat in app.Categories ?? Enumerable.Empty<string>())
            {
                var match = vm.ConfigFileContent.Categories!
                    .FirstOrDefault(c => c == cat);
                if (match != null)
                {
                    SelectedApp.SelectedItems?.Add(match);
                }
            }
        };
    }
    private async void Close(object? sender, RoutedEventArgs e)
    {
        await _mainVm.ReadCache();
        Close();
    }
    private async void UpdateApplication(object? sender, RoutedEventArgs e)
    {
        var app = new Application
        {
            Id = _application.Id,
            Name = AppName.Text,
            IconPath = IconFile.Text,
            AppImagePath = Executable.Text,
            Argument = NewAppArg.Text,
            RunTime = _application.RunTime,
            Categories = EditAppViewModel.SelectedCategories
        };
        Console.WriteLine(app.Id);
        _ = Cache.UpdateApplication(app);
        await _mainVm.ReadCache();
        Close();
    }

    private void SetSelectedCategories(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox categoryOverview)
        {
            if (categoryOverview.SelectedItems == null) return;
            var selectedCategories = categoryOverview.SelectedItems
                .OfType<object>() 
                .Select(item => item?.ToString())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            (DataContext as EditAppViewModel)?.SetSelectedCategories(selectedCategories);
        }
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
}