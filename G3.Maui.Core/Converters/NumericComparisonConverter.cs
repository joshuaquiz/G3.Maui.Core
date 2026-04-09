using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Converters;

/// <summary>
/// Converts numeric values to boolean based on a comparison.
/// Parameter format: "operator" or "operator:threshold" (threshold defaults to 0).
/// Supported operators: ==, !=, >, &lt;, >=, &lt;=
/// Examples: ConverterParameter=">" returns true if value > 0;
///           ConverterParameter="==:5" returns true if value == 5.
/// </summary>
public sealed class NumericComparisonConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value == null)
        {
            return false;
        }

        var paramStr = parameter?.ToString() ?? ">";
        var parts = paramStr.Split(':');
        var op = parts[0].Trim();
        var thresholdStr = parts.Length > 1 ? parts[1].Trim() : "0";

        decimal numericValue;

        try
        {
            numericValue = value switch
            {
                int i => i,
                double d => (decimal)d,
                decimal dec => dec,
                float f => (decimal)f,
                long l => l,
                _ => throw new ArgumentException($"Unsupported type: {value.GetType()}")
            };
        }
        catch
        {
            return false;
        }
        if (!decimal.TryParse(thresholdStr, out var threshold))
        {
            threshold = 0;
        }

        return op switch
        {
            "==" => numericValue == threshold,
            "!=" => numericValue != threshold,
            ">" => numericValue > threshold,
            "<" => numericValue < threshold,
            ">=" => numericValue >= threshold,
            "<=" => numericValue <= threshold,
            _ => false
        };
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}
