using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class CarouselPositionConverterTests
{
    private readonly CarouselPositionConverter _sut = new();

    [Fact]
    public void Convert_NegativeInt_ReturnsZero()
    {
        var result = _sut.Convert(-1, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.Equal(0, result);
    }

    [Fact]
    public void Convert_Zero_ReturnsZero()
    {
        var result = _sut.Convert(0, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.Equal(0, result);
    }

    [Fact]
    public void Convert_PositiveInt_ReturnsSameValue()
    {
        var result = _sut.Convert(3, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.Equal(3, result);
    }

    [Fact]
    public void Convert_NonInt_ReturnsSameValue()
    {
        var result = _sut.Convert("not an int", typeof(object), null, CultureInfo.InvariantCulture);

        Assert.Equal("not an int", result);
    }

    [Fact]
    public void Convert_Null_ReturnsNull()
    {
        var result = _sut.Convert(null, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_NegativeInt_ReturnsZero()
    {
        var result = _sut.ConvertBack(-5, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.Equal(0, result);
    }

    [Fact]
    public void ConvertBack_PositiveInt_ReturnsSameValue()
    {
        var result = _sut.ConvertBack(2, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.Equal(2, result);
    }
}
