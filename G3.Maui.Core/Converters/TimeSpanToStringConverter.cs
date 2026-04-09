using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Converters;

/// <summary>
/// Converts a <see cref="TimeSpan"/> to a human-readable string.
/// Output format: "2 h 30 mins", "2 h", or "45 mins".
/// </summary>
public sealed class TimeSpanToStringConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not TimeSpan ts)
        {
            return string.Empty;
        }

        var hours = (int)ts.TotalHours;
        var minutes = ts.Minutes;
        if (hours > 0)
        {
            return minutes > 0 ? $"{hours} h {minutes} mins" : $"{hours} h";
        }

        return $"{minutes} mins";
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}
