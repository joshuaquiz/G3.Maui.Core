namespace G3.Maui.Core.Exceptions;

public sealed class NoInternetException()
    : G3MauiCoreException(
        "Check your network connection and try again.");