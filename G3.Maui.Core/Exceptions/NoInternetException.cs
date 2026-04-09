namespace G3.Maui.Core.Exceptions;

/// <summary>
/// Thrown when an HTTP operation is attempted without an active internet connection.
/// Catch this to show a user-facing "no connection" message.
/// </summary>
public sealed class NoInternetException()
    : G3MauiCoreException(
        "Check your network connection and try again.");