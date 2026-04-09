using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Converters;

/// <summary>
/// Returns true if the numeric value is greater than zero.
/// Consider using <see cref="NumericComparisonConverter"/> with parameter=">" for new code.
/// </summary>
public sealed class IsGreaterThanZeroConverter : IValueConverter
{
    private static readonly NumericComparisonConverter _inner = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => _inner.Convert(value, targetType, ">", culture);

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}
