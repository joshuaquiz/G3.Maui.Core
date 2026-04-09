using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class IsNullOrEmptyConverterTests
{
    private readonly IsNullOrEmptyConverter _sut = new();

    [Fact]
    public void Convert_Null_ReturnsTrue()
    {
        var result = _sut.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_EmptyString_ReturnsTrue()
    {
        var result = _sut.Convert("", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_WhitespaceString_ReturnsTrue()
    {
        var result = _sut.Convert("   ", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_NonEmptyString_ReturnsFalse()
    {
        var result = _sut.Convert("hello", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(
            () => _sut.ConvertBack(true, typeof(string), null, CultureInfo.InvariantCulture));
    }
}
