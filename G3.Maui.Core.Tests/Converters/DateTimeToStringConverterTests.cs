using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class DateTimeToStringConverterTests
{
    private readonly DateTimeToStringConverter _sut = new();
    private static readonly DateTime _testDate = new(2025, 6, 15, 10, 30, 0);

    [Fact]
    public void Convert_DateTime_UsesDefaultFormat()
    {
        var result = _sut.Convert(_testDate, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal(_testDate.ToString("g", CultureInfo.InvariantCulture), result);
    }

    [Fact]
    public void Convert_DateTime_UsesCustomFormat()
    {
        _sut.Format = "yyyy-MM-dd";

        var result = _sut.Convert(_testDate, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal("2025-06-15", result);
    }

    [Fact]
    public void Convert_DateTime_ParameterOverridesFormat()
    {
        _sut.Format = "yyyy-MM-dd";

        var result = _sut.Convert(_testDate, typeof(string), "dd/MM/yyyy", CultureInfo.InvariantCulture);

        Assert.Equal("15/06/2025", result);
    }

    [Fact]
    public void Convert_NonDateTime_ReturnsEmpty()
    {
        var result = _sut.Convert("not a date", typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Convert_Null_ReturnsEmpty()
    {
        var result = _sut.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(
            () => _sut.ConvertBack("2025-01-01", typeof(DateTime), null, CultureInfo.InvariantCulture));
    }
}
