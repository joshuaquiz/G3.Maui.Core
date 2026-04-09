using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class BoolToDoubleConverterTests
{
    private readonly BoolToDoubleConverter _sut = new();

    [Fact]
    public void Convert_True_ReturnsTrueValue()
    {
        var result = _sut.Convert(true, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Convert_False_ReturnsFalseValue()
    {
        var result = _sut.Convert(false, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Convert_Null_ReturnsFalseValue()
    {
        var result = _sut.Convert(null, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Convert_NonBool_ReturnsFalseValue()
    {
        var result = _sut.Convert("not a bool", typeof(double), null, CultureInfo.InvariantCulture);

        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Convert_CustomTrueValue_ReturnsCustomTrue()
    {
        _sut.TrueValue = 0.5;
        _sut.FalseValue = 0.2;

        var result = _sut.Convert(true, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.Equal(0.5, result);
    }

    [Fact]
    public void Convert_CustomFalseValue_ReturnsCustomFalse()
    {
        _sut.TrueValue = 0.5;
        _sut.FalseValue = 0.2;

        var result = _sut.Convert(false, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.Equal(0.2, result);
    }

    [Fact]
    public void ConvertBack_TrueValue_ReturnsTrue()
    {
        var result = _sut.ConvertBack(1.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void ConvertBack_FalseValue_ReturnsFalse()
    {
        var result = _sut.ConvertBack(0.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_NonDouble_ReturnsFalse()
    {
        var result = _sut.ConvertBack("not a double", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_NearTrueValue_ReturnsTrue()
    {
        var result = _sut.ConvertBack(1.0005, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }
}
