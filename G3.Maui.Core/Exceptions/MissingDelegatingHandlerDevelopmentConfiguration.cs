using System.Net.Http;

namespace G3.Maui.Core.Exceptions;

/// <summary>
/// Thrown when a development delegating handler receives a request that has no matching
/// mocked configuration. Add the missing URL pattern and HTTP method to your
/// <see cref="G3.Maui.Core.Models.BaseDelegatingHandler"/> subclass.
/// </summary>
public sealed class MissingDelegatingHandlerDevelopmentConfiguration(
    string absoluteUri,
    HttpMethod requestMethod)
    : G3MauiCoreException(
        $"The development delegating handler configuration is missing for {requestMethod} {absoluteUri}.");