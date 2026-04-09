using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class NumericComparisonConverterTests
{
    private readonly NumericComparisonConverter _sut = new();

    [Theory]
    [InlineData(1, ">", true)]
    [InlineData(0, ">", false)]
    [InlineData(-1, ">", false)]
    public void Convert_GreaterThan_ReturnsExpected(int value, string op, bool expected)
    {
        var result = _sut.Convert(value, typeof(bool), op, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(-1, "<", true)]
    [InlineData(0, "<", false)]
    [InlineData(1, "<", false)]
    public void Convert_LessThan_ReturnsExpected(int value, string op, bool expected)
    {
        var result = _sut.Convert(value, typeof(bool), op, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, "==", true)]
    [InlineData(1, "==", false)]
    public void Convert_Equal_ReturnsExpected(int value, string op, bool expected)
    {
        var result = _sut.Convert(value, typeof(bool), op, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, "!=", true)]
    [InlineData(0, "!=", false)]
    public void Convert_NotEqual_ReturnsExpected(int value, string op, bool expected)
    {
        var result = _sut.Convert(value, typeof(bool), op, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, ">=", true)]
    [InlineData(1, ">=", true)]
    [InlineData(-1, ">=", false)]
    public void Convert_GreaterOrEqual_ReturnsExpected(int value, string op, bool expected)
    {
        var result = _sut.Convert(value, typeof(bool), op, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, "<=", true)]
    [InlineData(-1, "<=", true)]
    [InlineData(1, "<=", false)]
    public void Convert_LessOrEqual_ReturnsExpected(int value, string op, bool expected)
    {
        var result = _sut.Convert(value, typeof(bool), op, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_WithCustomThreshold_ComparesToThreshold()
    {
        var result = _sut.Convert(6, typeof(bool), ">:5", CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_WithCustomThreshold_BelowThreshold_ReturnsFalse()
    {
        var result = _sut.Convert(4, typeof(bool), ">:5", CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_Null_ReturnsFalse()
    {
        var result = _sut.Convert(null, typeof(bool), ">", CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_UnsupportedType_ReturnsFalse()
    {
        var result = _sut.Convert("not a number", typeof(bool), ">", CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_DoubleValue_ComparesCorrectly()
    {
        var result = _sut.Convert(3.5d, typeof(bool), ">", CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_LongValue_ComparesCorrectly()
    {
        var result = _sut.Convert(100L, typeof(bool), ">", CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_UnknownOperator_ReturnsFalse()
    {
        var result = _sut.Convert(1, typeof(bool), "???", CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_DefaultParameter_UsesGreaterThan()
    {
        var result = _sut.Convert(5, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(
            () => _sut.ConvertBack(true, typeof(int), null, CultureInfo.InvariantCulture));
    }
}
