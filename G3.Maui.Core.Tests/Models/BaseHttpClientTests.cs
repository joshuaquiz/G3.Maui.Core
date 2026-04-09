using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using G3.Maui.Core.Exceptions;
using G3.Maui.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Maui.Networking;
using NSubstitute;
using Xunit;

namespace G3.Maui.Core.Tests.Models;

public sealed class BaseHttpClientTests : IDisposable
{
    private readonly IConnectivity _connectivity = Substitute.For<IConnectivity>();
    private readonly MockHttpMessageHandler _messageHandler = new();
    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());
    private readonly TestableHttpClient _sut;

    public BaseHttpClientTests()
    {
        _sut = new TestableHttpClient(
            _connectivity,
            new HttpClient(_messageHandler),
            _memoryCache,
            NullLogger<BaseHttpClient>.Instance);
    }

    [Fact]
    public async Task Get_NoInternet_ThrowsNoInternetException()
    {
        _connectivity.NetworkAccess.Returns(NetworkAccess.None);

        await Assert.ThrowsAsync<NoInternetException>(
            () => _sut.Get<TestModel>(
                new Uri("https://api.test.com/items"),
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Get_WithInternet_ReturnsDeserializedResponse()
    {
        _connectivity.NetworkAccess.Returns(NetworkAccess.Internet);
        _messageHandler.SetResponse(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new TestModel { Id = 42, Name = "Test" })
            });

        var result = await _sut.Get<TestModel>(
            new Uri("https://api.test.com/items"),
            CancellationToken.None);

        Assert.Equal(42, result.Id);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task Get_SecondCall_ReturnsCachedResponse()
    {
        _connectivity.NetworkAccess.Returns(NetworkAccess.Internet);
        _messageHandler.SetResponse(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new TestModel { Id = 1, Name = "Cached" })
            });

        var first = await _sut.Get<TestModel>(
            new Uri("https://api.test.com/cached"),
            CancellationToken.None,
            TimeSpan.FromMinutes(1));

        _messageHandler.SetResponse(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new TestModel { Id = 99, Name = "New" })
            });

        var second = await _sut.Get<TestModel>(
            new Uri("https://api.test.com/cached"),
            CancellationToken.None,
            TimeSpan.FromMinutes(1));

        Assert.Equal(1, second.Id);
        Assert.Equal("Cached", second.Name);
    }

    [Fact]
    public async Task Post_NoInternet_ThrowsNoInternetException()
    {
        _connectivity.NetworkAccess.Returns(NetworkAccess.None);

        await Assert.ThrowsAsync<NoInternetException>(
            () => _sut.Post<TestModel, TestModel>(
                new Uri("https://api.test.com/items"),
                new TestModel { Id = 1 },
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Post_WithInternet_ReturnsDeserializedResponse()
    {
        _connectivity.NetworkAccess.Returns(NetworkAccess.Internet);
        _messageHandler.SetResponse(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new TestModel { Id = 10, Name = "Created" })
            });

        var result = await _sut.Post<TestModel, TestModel>(
            new Uri("https://api.test.com/items"),
            new TestModel { Id = 0, Name = "New" },
            CancellationToken.None);

        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task Put_NoInternet_ThrowsNoInternetException()
    {
        _connectivity.NetworkAccess.Returns(NetworkAccess.None);

        await Assert.ThrowsAsync<NoInternetException>(
            () => _sut.Put<TestModel, TestModel>(
                new Uri("https://api.test.com/items/1"),
                new TestModel { Id = 1 },
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Delete_NoInternet_ThrowsNoInternetException()
    {
        _connectivity.NetworkAccess.Returns(NetworkAccess.None);

        await Assert.ThrowsAsync<NoInternetException>(
            () => _sut.Delete<TestModel>(
                new Uri("https://api.test.com/items/1"),
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Patch_NoInternet_ThrowsNoInternetException()
    {
        _connectivity.NetworkAccess.Returns(NetworkAccess.None);

        await Assert.ThrowsAsync<NoInternetException>(
            () => _sut.Patch<TestModel, TestModel>(
                new Uri("https://api.test.com/items/1"),
                new TestModel { Id = 1 },
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Get_ConstrainedInternet_ReturnsResponse()
    {
        _connectivity.NetworkAccess.Returns(NetworkAccess.ConstrainedInternet);
        _messageHandler.SetResponse(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new TestModel { Id = 7, Name = "Constrained" })
            });

        var result = await _sut.Get<TestModel>(
            new Uri("https://api.test.com/constrained"),
            CancellationToken.None);

        Assert.Equal(7, result.Id);
    }

    public void Dispose()
    {
        _sut.Dispose();
        _memoryCache.Dispose();
        _messageHandler.Dispose();
    }

    private sealed class TestableHttpClient(
        IConnectivity connectivity,
        HttpClient httpClient,
        IMemoryCache memoryCache,
        Microsoft.Extensions.Logging.ILogger<BaseHttpClient> logger)
        : BaseHttpClient(connectivity, httpClient, memoryCache, logger)
    {
    }

    private sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage _response = new(HttpStatusCode.OK);

        public void SetResponse(
            HttpResponseMessage response)
            => _response = response;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => Task.FromResult(_response);
    }

    private sealed class TestModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
