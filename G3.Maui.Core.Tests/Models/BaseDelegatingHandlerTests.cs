using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using G3.Maui.Core.Exceptions;
using G3.Maui.Core.Models;
using Xunit;

namespace G3.Maui.Core.Tests.Models;

public sealed class BaseDelegatingHandlerTests
{
    [Fact]
    public async Task SendAsync_MatchingGetRequest_ReturnsOkWithContent()
    {
        var handler = new TestableDelegatingHandler(
            [
                new MockedHttpRequest(
                    new Regex("https://api\\.example\\.com/users"),
                    [
                        new MockedHttpRequestMethodActions(
                            HttpMethod.Get,
                            (_, _) => ValueTask.FromResult<HttpContent>(
                                new StringContent(
                                    "{\"id\":1}",
                                    Encoding.UTF8,
                                    "application/json")))
                    ])
            ]);
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api.example.com/users");

        var response = await handler.TestSendAsync(
            request,
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"id\":1}", body);
    }

    [Fact]
    public async Task SendAsync_NoMatchingUrl_ThrowsMissingConfiguration()
    {
        var handler = new TestableDelegatingHandler([]);
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api.example.com/missing");

        await Assert.ThrowsAsync<MissingDelegatingHandlerDevelopmentConfiguration>(
            () => handler.TestSendAsync(
                request,
                CancellationToken.None));
    }

    [Fact]
    public async Task SendAsync_UrlMatchesButWrongMethod_ThrowsMissingConfiguration()
    {
        var handler = new TestableDelegatingHandler(
            [
                new MockedHttpRequest(
                    new Regex("https://api\\.example\\.com/users"),
                    [
                        new MockedHttpRequestMethodActions(
                            HttpMethod.Get,
                            (_, _) => ValueTask.FromResult<HttpContent>(
                                new StringContent("{}")))
                    ])
            ]);
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.example.com/users");

        await Assert.ThrowsAsync<MissingDelegatingHandlerDevelopmentConfiguration>(
            () => handler.TestSendAsync(
                request,
                CancellationToken.None));
    }

    [Fact]
    public async Task SendAsync_RegexMatchesSubstring_ReturnsOk()
    {
        var handler = new TestableDelegatingHandler(
            [
                new MockedHttpRequest(
                    new Regex("/users/\\d+"),
                    [
                        new MockedHttpRequestMethodActions(
                            HttpMethod.Get,
                            (_, _) => ValueTask.FromResult<HttpContent>(
                                new StringContent("{\"id\":42}")))
                    ])
            ]);
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api.example.com/users/42");

        var response = await handler.TestSendAsync(
            request,
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SendAsync_MultipleHandlers_MatchesCorrectOne()
    {
        var handler = new TestableDelegatingHandler(
            [
                new MockedHttpRequest(
                    new Regex("/products"),
                    [
                        new MockedHttpRequestMethodActions(
                            HttpMethod.Get,
                            (_, _) => ValueTask.FromResult<HttpContent>(
                                new StringContent("products")))
                    ]),
                new MockedHttpRequest(
                    new Regex("/users"),
                    [
                        new MockedHttpRequestMethodActions(
                            HttpMethod.Get,
                            (_, _) => ValueTask.FromResult<HttpContent>(
                                new StringContent("users")))
                    ])
            ]);
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api.example.com/users");

        var response = await handler.TestSendAsync(
            request,
            CancellationToken.None);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("users", body);
    }

    private sealed class TestableDelegatingHandler(
        List<MockedHttpRequest> mockedHttpRequests)
        : BaseDelegatingHandler(mockedHttpRequests)
    {
        public Task<HttpResponseMessage> TestSendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => SendAsync(
                request,
                cancellationToken);
    }
}
