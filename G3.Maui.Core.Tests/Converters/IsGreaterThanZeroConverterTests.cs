using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class IsGreaterThanZeroConverterTests
{
    private readonly IsGreaterThanZeroConverter _sut = new();

    [Fact]
    public void Convert_PositiveInt_ReturnsTrue()
    {
        var result = _sut.Convert(5, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_Zero_ReturnsFalse()
    {
        var result = _sut.Convert(0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NegativeInt_ReturnsFalse()
    {
        var result = _sut.Convert(-1, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_PositiveDouble_ReturnsTrue()
    {
        var result = _sut.Convert(0.5d, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_Null_ReturnsFalse()
    {
        var result = _sut.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(
            () => _sut.ConvertBack(true, typeof(int), null, CultureInfo.InvariantCulture));
    }
}
