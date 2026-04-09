using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class NullToBooleanConverterTests
{
    private readonly NullToBooleanConverter _sut = new();

    [Fact]
    public void Convert_Null_ReturnsTrue()
    {
        var result = _sut.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_NonNull_ReturnsFalse()
    {
        var result = _sut.Convert("something", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_Null_WithInvert_ReturnsFalse()
    {
        var result = _sut.Convert(null, typeof(bool), "Invert", CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NonNull_WithInvert_ReturnsTrue()
    {
        var result = _sut.Convert("something", typeof(bool), "Invert", CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_Null_WithInvertCaseInsensitive_ReturnsFalse()
    {
        var result = _sut.Convert(null, typeof(bool), "invert", CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(
            () => _sut.ConvertBack(true, typeof(object), null, CultureInfo.InvariantCulture));
    }
}
