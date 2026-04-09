using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Converters;

/// <summary>
/// Returns true if the collection is not null and not empty.
/// Supports <see cref="ICollection"/> (uses Count) and <see cref="IEnumerable"/> (uses Any()).
/// </summary>
public sealed class CollectionNotEmptyToBoolConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is ICollection col)
        {
            return col.Count > 0;
        }
        if (value is IEnumerable en)
        {
            return en.Cast<object>().Any();
        }

        return false;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}
