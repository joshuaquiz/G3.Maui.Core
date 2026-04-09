using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class StringNotEmptyToBoolConverterTests
{
    private readonly StringNotEmptyToBoolConverter _sut = new();

    [Fact]
    public void Convert_NonEmptyString_ReturnsTrue()
    {
        var result = _sut.Convert("hello", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_EmptyString_ReturnsFalse()
    {
        var result = _sut.Convert("", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_Null_ReturnsFalse()
    {
        var result = _sut.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_WhitespaceOnly_ReturnsFalse()
    {
        var result = _sut.Convert("   ", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(
            () => _sut.ConvertBack(true, typeof(string), null, CultureInfo.InvariantCulture));
    }
}
