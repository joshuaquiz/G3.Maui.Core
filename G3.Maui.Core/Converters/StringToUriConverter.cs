using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Converters;

/// <summary>
/// Converts a string URL to a <see cref="Uri"/> object.
/// Returns null if the string is null, empty, or not a valid absolute URI.
/// ConvertBack returns the URI's string representation.
/// </summary>
public sealed class StringToUriConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is string s && !string.IsNullOrWhiteSpace(s) && Uri.TryCreate(s, UriKind.Absolute, out var uri))
        {
            return uri;
        }

        return null;
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value?.ToString();
}
