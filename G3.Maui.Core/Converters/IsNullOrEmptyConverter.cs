using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Converters;

/// <summary>
/// Returns true if the value is null or whitespace, false otherwise.
/// </summary>
public sealed class IsNullOrEmptyConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => string.IsNullOrWhiteSpace(value as string);

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}
