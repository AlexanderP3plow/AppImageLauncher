using Avalonia;
using Avalonia.Styling;

namespace AppImageLauncher.Controller;

public static class ThemeManager
{
    private static ThemeVariant _currentTheme = ThemeVariant.Default;

    public static ThemeVariant CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;

                if (Application.Current != null)
                    Application.Current.RequestedThemeVariant = _currentTheme;
            }
        }
    }
    
    public static void Toggle()
    {
        CurrentTheme = CurrentTheme == ThemeVariant.Light
            ? ThemeVariant.Light
            : ThemeVariant.Default;
    }
}