using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Converters;

/// <summary>
/// Ensures a CarouselView position is never set to -1 (which crashes on Android).
/// Returns 0 for any negative value, otherwise the original value.
/// </summary>
public sealed class CarouselPositionConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is int p && p < 0 ? 0 : value;

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is int p && p < 0 ? 0 : value;
}
