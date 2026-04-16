using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using G3.Maui.Core.Models;
using Xunit;

namespace G3.Maui.Core.Tests.Models;

public sealed class MockedHttpRequestTests
{
    [Fact]
    public void MockedHttpRequest_StoresUriPatternAndActions()
    {
        var pattern = new Regex("/api/users");
        var actions = new List<MockedHttpRequestMethodActions>
        {
            new(HttpMethod.Get, (_, _) => ValueTask.FromResult<HttpContent>(new StringContent("{}")))
        };

        var request = new MockedHttpRequest(pattern, actions);

        Assert.Equal(pattern, request.UriPattern);
        Assert.Equal(actions, request.HttpMethodActions);
    }

    [Fact]
    public void MockedHttpRequestMethodActions_StoresMethodAndFunc()
    {
        Func<HttpRequestMessage, CancellationToken, ValueTask<HttpContent>> func =
            (_, _) => ValueTask.FromResult<HttpContent>(new StringContent("{}"));

        var actions = new MockedHttpRequestMethodActions(HttpMethod.Post, func);

        Assert.Equal(HttpMethod.Post, actions.HttpMethod);
        Assert.Equal(func, actions.GenerateContentFunc);
    }

    [Fact]
    public async Task CreateStringContent_ReturnsJsonContentType()
    {
        var content = TestableDelegatingHandler.InvokeCreateStringContent("{\"id\":1}");

        Assert.Equal("application/json", content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task CreateStringContent_ReturnsUtf8Encoding()
    {
        var content = TestableDelegatingHandler.InvokeCreateStringContent("hello");

        Assert.Equal("utf-8", content.Headers.ContentType?.CharSet);
    }

    [Fact]
    public async Task CreateStringContent_PreservesBody()
    {
        const string body = "{\"name\":\"test\",\"value\":42}";

        var content = TestableDelegatingHandler.InvokeCreateStringContent(body);

        Assert.Equal(body, await content.ReadAsStringAsync());
    }

    [Fact]
    public void NavBarCommand_DefaultValues_AreEmpty()
    {
        var cmd = new NavBarCommand();

        Assert.Equal(string.Empty, cmd.IconSource);
        Assert.Equal(string.Empty, cmd.DisplayName);
        Assert.Null(cmd.Command);
        Assert.Null(cmd.CommandParameter);
    }

    [Fact]
    public void NavBarCommand_PropertiesCanBeSet()
    {
        var cmd = new NavBarCommand
        {
            IconSource = "gear",
            DisplayName = "Settings",
            CommandParameter = 42
        };

        Assert.Equal("gear", cmd.IconSource);
        Assert.Equal("Settings", cmd.DisplayName);
        Assert.Equal(42, cmd.CommandParameter);
    }

    private sealed class TestableDelegatingHandler(
        List<MockedHttpRequest> mockedHttpRequests)
        : BaseDelegatingHandler(mockedHttpRequests)
    {
        public static System.Net.Http.StringContent InvokeCreateStringContent(string content)
            => CreateStringContent(content);
    }
}
