using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Converters;

/// <summary>
/// Returns true if the bound value equals the converter parameter.
/// Numeric values are compared as integers when both sides parse as int;
/// otherwise a string comparison is used.
/// Example: ConverterParameter="2" returns true when the value is 2.
/// </summary>
public sealed class EqualToConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return false;
        }
        if (int.TryParse(value.ToString(), out var iv) && int.TryParse(parameter.ToString(), out var ip))
        {
            return iv == ip;
        }

        return value.ToString() == parameter.ToString();
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}
