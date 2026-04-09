using System.Net.Http;
using G3.Maui.Core.Exceptions;
using Xunit;

namespace G3.Maui.Core.Tests.Exceptions;

public sealed class ExceptionTests
{
    [Fact]
    public void NoInternetException_IsG3MauiCoreException()
    {
        var ex = new NoInternetException();

        Assert.IsAssignableFrom<G3MauiCoreException>(ex);
    }

    [Fact]
    public void NoInternetException_HasExpectedMessage()
    {
        var ex = new NoInternetException();

        Assert.Equal("Check your network connection and try again.", ex.Message);
    }

    [Fact]
    public void MissingDelegatingHandlerDevelopmentConfiguration_IsG3MauiCoreException()
    {
        var ex = new MissingDelegatingHandlerDevelopmentConfiguration(
            "https://api.example.com/users",
            HttpMethod.Get);

        Assert.IsAssignableFrom<G3MauiCoreException>(ex);
    }

    [Fact]
    public void MissingDelegatingHandlerDevelopmentConfiguration_MessageContainsMethodAndUrl()
    {
        var ex = new MissingDelegatingHandlerDevelopmentConfiguration(
            "https://api.example.com/users",
            HttpMethod.Post);

        Assert.Contains("POST", ex.Message);
        Assert.Contains("https://api.example.com/users", ex.Message);
    }

    [Fact]
    public void G3MauiCoreException_IsSystemException()
    {
        var ex = new NoInternetException();

        Assert.IsAssignableFrom<System.Exception>(ex);
    }
}
