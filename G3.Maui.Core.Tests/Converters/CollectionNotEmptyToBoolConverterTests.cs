using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using G3.Maui.Core.Converters;
using Xunit;

namespace G3.Maui.Core.Tests.Converters;

public sealed class CollectionNotEmptyToBoolConverterTests
{
    private readonly CollectionNotEmptyToBoolConverter _sut = new();

    [Fact]
    public void Convert_EmptyList_ReturnsFalse()
    {
        var result = _sut.Convert(new List<string>(), typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NonEmptyList_ReturnsTrue()
    {
        var result = _sut.Convert(new List<string> { "a" }, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_NullValue_ReturnsFalse()
    {
        var result = _sut.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_EmptyEnumerable_ReturnsFalse()
    {
        var result = _sut.Convert(Enumerable.Empty<string>(), typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_NonEmptyEnumerable_ReturnsTrue()
    {
        var result = _sut.Convert(new[] { 1, 2, 3 }.Select(x => x), typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_NonCollection_ReturnsFalse()
    {
        var result = _sut.Convert(42, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(
            () => _sut.ConvertBack(true, typeof(object), null, CultureInfo.InvariantCulture));
    }
}
