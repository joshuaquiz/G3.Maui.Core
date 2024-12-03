using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using G3.Maui.Core.Exceptions;

namespace G3.Maui.Core.Models;

/// <summary>
/// A base <see cref="DelegatingHandler"/> used in development to mock HTTP responses.
/// </summary>
/// <param name="mockedHttpRequests">A list of mocked HTTP responses.</param>
public abstract class BaseDelegatingHandler(
    List<MockedHttpRequest> mockedHttpRequests)
    : DelegatingHandler
{
    /// <summary>
    /// Gets a <see cref="ValueTask{T}"/> of <see cref="HttpContent"/> for a given <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequestMessage"/> to match to.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="ValueTask{T}"/> of <see cref="HttpContent"/>.</returns>
    /// <exception cref="MissingDelegatingHandlerDevelopmentConfiguration">Thrown in a missing request is asked for.</exception>
    protected virtual async ValueTask<HttpContent> GetHttpContent(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var resultFunction = mockedHttpRequests
            .FirstOrDefault(x =>
                x.UriPattern.IsMatch(
                        request.RequestUri?.OriginalString
                        ?? string.Empty))
            ?.HttpMethodActions
            .FirstOrDefault(x =>
                x.HttpMethod == request.Method)
            ?.GenerateContentFunc(
                request,
                cancellationToken);
        if (!resultFunction.HasValue)
        {
            throw new MissingDelegatingHandlerDevelopmentConfiguration(
                request.RequestUri!.AbsoluteUri,
                request.Method);
        }

        return await resultFunction.Value;
    }

    /// <inheritdoc />
    /// <exception cref="MissingDelegatingHandlerDevelopmentConfiguration">Thrown in a missing request is asked for.</exception>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken) =>
        new()
        {
            StatusCode = HttpStatusCode.OK,
            Content = await GetHttpContent(
                request,
                cancellationToken)
        };

    /// <summary>
    /// Wraps a <see cref="string"/> with a UTF-8 application/json <see cref="StringContent"/>.
    /// </summary>
    /// <param name="content">The string to wrap.</param>
    /// <returns>A UTF-8 application/json <see cref="StringContent"/> of the original <see cref="string"/>.</returns>
    protected static StringContent CreateStringContent(
        string content) =>
        new(
            content,
            Encoding.UTF8,
            "application/json");
}