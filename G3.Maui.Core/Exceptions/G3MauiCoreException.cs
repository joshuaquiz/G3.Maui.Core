using System;

namespace G3.Maui.Core.Exceptions;

/// <summary>
/// Base exception type for all G3.Maui.Core library exceptions.
/// Catch this type to handle any error originating from the library.
/// </summary>
public abstract class G3MauiCoreException : Exception
{
    /// <summary>Initializes a new instance of <see cref="G3MauiCoreException"/>.</summary>
    protected G3MauiCoreException()
    {
    }

    /// <summary>Initializes a new instance of <see cref="G3MauiCoreException"/>.</summary>
    /// <param name="message">The message that describes the error.</param>
    protected G3MauiCoreException(
        string message)
        : base(
            message)
    {
    }

    /// <summary>Initializes a new instance of <see cref="G3MauiCoreException"/>.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of this exception.</param>
    protected G3MauiCoreException(
        string message,
        Exception innerException)
        : base(
            message,
            innerException)
    {
    }
}