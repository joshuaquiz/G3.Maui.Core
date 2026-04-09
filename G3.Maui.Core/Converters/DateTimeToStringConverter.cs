using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Converters;

/// <summary>
/// Converts a <see cref="DateTime"/> to a formatted string.
/// Set <see cref="Format"/> to any standard or custom .NET date/time format string.
/// The converter parameter, if supplied, takes precedence over <see cref="Format"/>,
/// allowing per-binding overrides without a second converter instance.
/// Defaults to "g" (short date and time, culture-aware).
/// Example: Format="MMM dd, yyyy" produces "Apr 09, 2026".
/// </summary>
public sealed class DateTimeToStringConverter : IValueConverter
{
    /// <summary>
    /// The format string used when no converter parameter is provided.
    /// Defaults to "g" (short date/time). Any standard or custom .NET format string is accepted.
    /// </summary>
    public string Format { get; set; } = "g";

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not DateTime dt)
        {
            return string.Empty;
        }

        var format = parameter as string ?? Format;
        return dt.ToString(format, culture);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}
