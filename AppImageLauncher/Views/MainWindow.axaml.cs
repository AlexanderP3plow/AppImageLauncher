using System;
using System.Diagnostics;
using System.Linq;
using AppImageLauncher.Classes;
using AppImageLauncher.Controller;
using AppImageLauncher.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace AppImageLauncher.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainWindowViewModel();
        (DataContext as MainWindowViewModel)?.ReadConfigFile();
        InitializeComponent();
        Loaded += SetDefaultView;
        Loaded += SetToggleButtons;
    }

    private async void SetDefaultView(object? sender, EventArgs e)
    {
        try
        {
            var mainWindowViewModel = new MainWindowViewModel();
            await mainWindowViewModel.ReadCache();
            AppsOverview.ItemsSource = mainWindowViewModel.ApplicationOverview;
        }
        catch (Exception err)
        {
            Console.WriteLine(err);
        }
    }

    private void SwitchTheme(object? sender, RoutedEventArgs e)
    {
        ThemeManager.Toggle();
        if (ThemeManager.CurrentTheme != ThemeVariant.Light)
        {
            Moon.IsVisible = true;
            Sun.IsVisible = false;
        }
        else
        {
            Moon.IsVisible = false;
            Sun.IsVisible = true;
        }
    }
    private void SetToggleButtons(object? sender, RoutedEventArgs e)
    {
        ThemeToggleButton.IsChecked = ThemeManager.CurrentTheme != ThemeVariant.Light;
        if (ThemeManager.CurrentTheme != ThemeVariant.Light)
        {
            Moon.IsVisible = true;
            Sun.IsVisible = false;
        }
        else
        {
            Moon.IsVisible = false;
            Sun.IsVisible = true;
        }
    }
    private void SetCategorySelection(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox categoryOverview)
        {
            if (categoryOverview.SelectedItems == null) return;
            var selectedCategories = categoryOverview.SelectedItems
                .OfType<object>() 
                .Select(item => item?.ToString())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            (DataContext as MainWindowViewModel)?.SetSelectedCategories(selectedCategories);
        }
    }

    private void OpenNewApp(object? sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        var newApp = new NewApp(viewModel!);
        newApp.Show();
    }
    private async void StartApplication(object? sender, RoutedEventArgs e)
    {
        if (sender is Button appContainer && appContainer.DataContext is Application app)
        {
            app.State = AppState.Running;
            var stopwatch = Stopwatch.StartNew();
            int? exitCode = await Starter.RunApp(app);
            stopwatch.Stop();
            app.RunTime = stopwatch.Elapsed; 
            app.State = exitCode == 0 ? AppState.Idle : AppState.Error;
            await Cache.UpdateRunTime(app);
            (DataContext as MainWindowViewModel)?.ReadCache();
        }
    }
    private void OpenEditApp(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && menuItem.DataContext is Application app)
        {
            var viewModel = DataContext as MainWindowViewModel;
            var editApp = new EditApp(viewModel!, app);
            editApp.Show(); 
        }
    }
}