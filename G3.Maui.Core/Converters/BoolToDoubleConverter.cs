using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Converters;

/// <summary>
/// Converts a boolean to one of two double values.
/// Set <see cref="TrueValue"/> and <see cref="FalseValue"/> to control the output.
/// Useful for opacity, scale, or other numeric properties driven by a flag.
/// </summary>
public class BoolToDoubleConverter : IValueConverter
{
    /// <summary>Value returned when the binding value is true. Defaults to 1.0.</summary>
    public double TrueValue { get; set; } = 1.0;

    /// <summary>Value returned when the binding value is false. Defaults to 0.0.</summary>
    public double FalseValue { get; set; } = 0.0;

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? TrueValue : FalseValue;
        }

        return FalseValue;
    }

    /// <inheritdoc />
    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is double d)
        {
            return Math.Abs(d - TrueValue) < 0.001;
        }

        return false;
    }
}
