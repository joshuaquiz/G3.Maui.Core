using System.Net.Http;

namespace G3.Maui.Core.Exceptions;

public sealed class MissingDelegatingHandlerDevelopmentConfiguration(
    string absoluteUri,
    HttpMethod requestMethod)
    : G3MauiCoreException(
        $"The development delegating handler configuration is missing for {requestMethod} {absoluteUri}.");