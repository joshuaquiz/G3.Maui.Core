using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Converters;

/// <summary>
/// Returns true when the bound value is equal to its type's default/zero value.
/// Supports <see cref="TimeSpan"/>, <see cref="int"/>, reference types (null check),
/// and any other value type (compared against <see cref="Activator.CreateInstance(Type)"/>).
/// An optional converter parameter sets a custom minimum threshold for TimeSpan and int:
/// e.g., ConverterParameter="60" with a TimeSpan returns true when TotalSeconds is 60 or less.
/// </summary>
public sealed class DefaultValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        switch (value)
        {
            case TimeSpan t:
            {
                var min = parameter == null ? TimeSpan.Zero : TimeSpan.FromSeconds(int.Parse((string)parameter));
                return t <= min;
            }
            case int i:
            {
                var min = parameter == null ? 0 : int.Parse((string)parameter);
                return i <= min;
            }
        }
        if (!targetType.IsValueType)
        {
            return value == null;
        }

        return value == Activator.CreateInstance(targetType);
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}
