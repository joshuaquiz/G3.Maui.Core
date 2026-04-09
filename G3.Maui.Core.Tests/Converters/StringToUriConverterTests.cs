using System;
using System.Globalization;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class StringToUriConverterTests
{
    private readonly StringToUriConverter _sut = new();

    [Fact]
    public void Convert_ValidAbsoluteUrl_ReturnsUri()
    {
        var result = _sut.Convert("https://example.com/path", typeof(Uri), null, CultureInfo.InvariantCulture);

        var uri = Assert.IsType<Uri>(result);
        Assert.Equal("https://example.com/path", uri.AbsoluteUri);
    }

    [Fact]
    public void Convert_NullString_ReturnsNull()
    {
        var result = _sut.Convert(null, typeof(Uri), null, CultureInfo.InvariantCulture);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_EmptyString_ReturnsNull()
    {
        var result = _sut.Convert("", typeof(Uri), null, CultureInfo.InvariantCulture);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_WhitespaceString_ReturnsNull()
    {
        var result = _sut.Convert("   ", typeof(Uri), null, CultureInfo.InvariantCulture);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_RelativeUrl_ReturnsNull()
    {
        var result = _sut.Convert("/relative/path", typeof(Uri), null, CultureInfo.InvariantCulture);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_InvalidUrl_ReturnsNull()
    {
        var result = _sut.Convert("not a url at all $$$$", typeof(Uri), null, CultureInfo.InvariantCulture);

        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_Uri_ReturnsStringRepresentation()
    {
        var uri = new Uri("https://example.com/path");

        var result = _sut.ConvertBack(uri, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal("https://example.com/path", result);
    }

    [Fact]
    public void ConvertBack_Null_ReturnsNull()
    {
        var result = _sut.ConvertBack(null, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Null(result);
    }
}
