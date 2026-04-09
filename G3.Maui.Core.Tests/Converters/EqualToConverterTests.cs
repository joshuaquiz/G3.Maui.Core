using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class EqualToConverterTests
{
    private readonly EqualToConverter _sut = new();

    [Fact]
    public void Convert_EqualIntegers_ReturnsTrue()
    {
        var result = _sut.Convert(2, typeof(bool), "2", CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_UnequalIntegers_ReturnsFalse()
    {
        var result = _sut.Convert(3, typeof(bool), "2", CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_EqualStrings_ReturnsTrue()
    {
        var result = _sut.Convert("hello", typeof(bool), "hello", CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_UnequalStrings_ReturnsFalse()
    {
        var result = _sut.Convert("hello", typeof(bool), "world", CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NullValue_ReturnsFalse()
    {
        var result = _sut.Convert(null, typeof(bool), "2", CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NullParameter_ReturnsFalse()
    {
        var result = _sut.Convert("hello", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_IntValueAndStringParam_ComparesAsIntegers()
    {
        var result = _sut.Convert(5, typeof(bool), "5", CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(
            () => _sut.ConvertBack(true, typeof(object), null, CultureInfo.InvariantCulture));
    }
}
