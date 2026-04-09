using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class DefaultValueConverterTests
{
    private readonly DefaultValueConverter _sut = new();

    [Fact]
    public void Convert_ZeroTimeSpan_ReturnsTrue()
    {
        var result = _sut.Convert(TimeSpan.Zero, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_NonZeroTimeSpan_ReturnsFalse()
    {
        var result = _sut.Convert(TimeSpan.FromMinutes(5), typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_TimeSpan_WithThreshold_AtThreshold_ReturnsTrue()
    {
        var result = _sut.Convert(TimeSpan.FromSeconds(60), typeof(bool), "60", CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_TimeSpan_WithThreshold_AboveThreshold_ReturnsFalse()
    {
        var result = _sut.Convert(TimeSpan.FromSeconds(61), typeof(bool), "60", CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_ZeroInt_ReturnsTrue()
    {
        var result = _sut.Convert(0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_NonZeroInt_ReturnsFalse()
    {
        var result = _sut.Convert(5, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_Int_WithThreshold_AtThreshold_ReturnsTrue()
    {
        var result = _sut.Convert(10, typeof(bool), "10", CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_Int_WithThreshold_AboveThreshold_ReturnsFalse()
    {
        var result = _sut.Convert(11, typeof(bool), "10", CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NullReferenceType_ReturnsTrue()
    {
        var result = _sut.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_NonNullReferenceType_ReturnsFalse()
    {
        var result = _sut.Convert("hello", typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(
            () => _sut.ConvertBack(true, typeof(int), null, CultureInfo.InvariantCulture));
    }
}
