using System;
using System.Globalization;
using AppImageLauncher.Classes;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AppImageLauncher.Controller;

public class AppStateToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            AppState.Running => Brushes.GreenYellow,
            AppState.Error => Brushes.DarkRed,
            _ => Brushes.DarkRed
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}