using Microsoft.Maui.Networking;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using G3.Maui.Core.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace G3.Maui.Core.Models;

/// <summary>
/// A base class for handling HTTP requests.
/// </summary>
/// <param name="connectivity"></param>
/// <param name="httpClient"></param>
/// <param name="memoryCache"></param>
public abstract class BaseHttpClient(
    IConnectivity connectivity,
    HttpClient httpClient,
    IMemoryCache memoryCache)
    : HttpClient
{
    private readonly SemaphoreSlim _lookupSemaphore = new(1);
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<HttpMethod, SemaphoreSlim>> _semaphoreSlims = new();

    protected bool HasInternet() =>
        connectivity.NetworkAccess
            is NetworkAccess.Internet
            or NetworkAccess.ConstrainedInternet;

    public async ValueTask<TResponse> Get<TResponse>(
        Uri url,
        CancellationToken cancellationToken,
        TimeSpan? cacheExpiry = null)
    {
        await SemaphoreSetup(
            HttpMethod.Get,
            url,
            cancellationToken);
        await _semaphoreSlims[url.AbsolutePath][HttpMethod.Get].WaitAsync(
            cancellationToken);
        try
        {
            return await memoryCache.GetOrCreateAsync(
                       url,
                       async item =>
                       {
                           try
                           {
                               if (!HasInternet())
                               {
                                   throw new NoInternetException();
                               }

                               var result = await httpClient.GetFromJsonAsync<TResponse>(
                                                url,
                                                cancellationToken)
                                            ?? throw new Exception(
                                                "No data returned");
                               item.Value = result;
                               item.AbsoluteExpirationRelativeToNow = cacheExpiry ?? TimeSpan.FromSeconds(3);
                               return result;
                           }
                           catch
                           {
                               item.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                               throw;
                           }
                       })
                   ?? throw new Exception(
                       "No data returned");
        }
        finally
        {
            _semaphoreSlims[url.AbsolutePath][HttpMethod.Get].Release(
                1);
        }
    }

    public async ValueTask<TResponse> Post<TResponse, TRequest>(
        Uri url,
        TRequest data,
        CancellationToken cancellationToken) =>
        await DataModificationInternal<TResponse, TRequest>(
            url,
            HttpMethod.Post,
            data,
            cancellationToken);

    public async ValueTask<TResponse> Put<TResponse, TRequest>(
        Uri url,
        TRequest data,
        CancellationToken cancellationToken) =>
        await DataModificationInternal<TResponse, TRequest>(
            url,
            HttpMethod.Put,
            data,
            cancellationToken);

    public async ValueTask<TResponse> Delete<TResponse>(
        Uri url,
        CancellationToken cancellationToken) =>
        await DataModificationInternal<TResponse, object>(
            url,
            HttpMethod.Delete,
            null,
            cancellationToken);

    public async ValueTask<TResponse> Patch<TResponse, TRequest>(
        Uri url,
        TRequest data,
        CancellationToken cancellationToken) =>
        await DataModificationInternal<TResponse, TRequest>(
            url,
            HttpMethod.Patch,
            data,
            cancellationToken);

    protected virtual async ValueTask<TResponse> DataModificationInternal<TResponse, TRequest>(
        Uri url,
        HttpMethod httpMethod,
        TRequest? data,
        CancellationToken cancellationToken)
    {
        if (!HasInternet())
        {
            throw new NoInternetException();
        }

        await SemaphoreSetup(
            httpMethod,
            url,
            cancellationToken);
        await _semaphoreSlims[url.AbsolutePath][httpMethod].WaitAsync(
            cancellationToken);
        try
        {
            var result = await httpClient.SendAsync(
                new HttpRequestMessage
                {
                    RequestUri = url,
                    Method = httpMethod,
                    Content = data == null
                        ? null
                        : JsonContent.Create(
                            data)
                },
                cancellationToken);
            var resultModel = await result.Content.ReadFromJsonAsync<TResponse>(
                cancellationToken: cancellationToken);
            if (resultModel == null)
            {
                throw new Exception(
                    "No data returned");
            }
            else
            {
                memoryCache.Remove(
                    url.AbsolutePath);
                return resultModel;
            }
        }
        finally
        {
            _semaphoreSlims[url.AbsolutePath][httpMethod].Release(
                1);
        }
    }

    protected async Task SemaphoreSetup(
        HttpMethod httpMethod,
        Uri url,
        CancellationToken cancellationToken)
    {
        await _lookupSemaphore.WaitAsync(
            cancellationToken);
        if (!_semaphoreSlims.ContainsKey(
                url.AbsolutePath))
        {
            _semaphoreSlims.TryAdd(
                url.AbsolutePath,
                new ConcurrentDictionary<HttpMethod, SemaphoreSlim>());
        }

        if (!_semaphoreSlims[url.AbsolutePath].ContainsKey(
                httpMethod))
        {
            _semaphoreSlims[url.AbsolutePath].TryAdd(
                httpMethod,
                new SemaphoreSlim(1));
        }

        _lookupSemaphore.Release(
            1);
    }
}