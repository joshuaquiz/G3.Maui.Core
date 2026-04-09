using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class TimeSpanToStringConverterTests
{
    private readonly TimeSpanToStringConverter _sut = new();

    [Fact]
    public void Convert_HoursAndMinutes_ReturnsHoursAndMinsFormat()
    {
        var result = _sut.Convert(TimeSpan.FromMinutes(150), typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal("2 h 30 mins", result);
    }

    [Fact]
    public void Convert_HoursOnly_ReturnsHoursFormat()
    {
        var result = _sut.Convert(TimeSpan.FromHours(3), typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal("3 h", result);
    }

    [Fact]
    public void Convert_MinutesOnly_ReturnsMinutesFormat()
    {
        var result = _sut.Convert(TimeSpan.FromMinutes(45), typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal("45 mins", result);
    }

    [Fact]
    public void Convert_ZeroMinutes_ReturnsZeroMins()
    {
        var result = _sut.Convert(TimeSpan.Zero, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal("0 mins", result);
    }

    [Fact]
    public void Convert_NonTimeSpan_ReturnsEmpty()
    {
        var result = _sut.Convert("not a timespan", typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Convert_Null_ReturnsEmpty()
    {
        var result = _sut.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Convert_OneHourThirtyMins_ReturnsCorrectFormat()
    {
        var result = _sut.Convert(TimeSpan.FromMinutes(90), typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal("1 h 30 mins", result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(
            () => _sut.ConvertBack("1 h 30 mins", typeof(TimeSpan), null, CultureInfo.InvariantCulture));
    }
}
