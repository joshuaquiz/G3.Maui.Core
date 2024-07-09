using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace G3.Maui.Core.Models;

/// <summary>
/// Handles converting an <see cref="HttpRequestMessage"/> into an <see cref="HttpContent"/> for a given HTTP method.
/// </summary>
/// <param name="HttpMethod">The HTTP method.</param>
/// <param name="GenerateContentFunc">The function to generate the response.</param>
public sealed record MockedHttpRequestMethodActions(
    HttpMethod HttpMethod,
    Func<HttpRequestMessage, CancellationToken, ValueTask<HttpContent>> GenerateContentFunc);