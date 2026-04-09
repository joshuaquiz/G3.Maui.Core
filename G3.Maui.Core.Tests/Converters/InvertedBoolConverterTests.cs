using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class InvertedBoolConverterTests
{
    private readonly InvertedBoolConverter _sut = new();

    [Fact]
    public void Convert_True_ReturnsFalse()
    {
        var result = _sut.Convert(true, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_False_ReturnsTrue()
    {
        var result = _sut.Convert(false, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_Null_ReturnsTrue()
    {
        var result = _sut.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_NonBool_ReturnsTrue()
    {
        var result = _sut.Convert("not a bool", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void ConvertBack_True_ReturnsFalse()
    {
        var result = _sut.ConvertBack(true, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_False_ReturnsTrue()
    {
        var result = _sut.ConvertBack(false, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void ConvertBack_NonBool_ReturnsFalse()
    {
        var result = _sut.ConvertBack("not a bool", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }
}
