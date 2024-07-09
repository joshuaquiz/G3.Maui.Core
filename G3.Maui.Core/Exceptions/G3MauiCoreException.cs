using System;

namespace G3.Maui.Core.Exceptions;

public abstract class G3MauiCoreException : Exception
{
    protected G3MauiCoreException()
    {
    }

    protected G3MauiCoreException(
        string message)
        : base(
            message)
    {
    }

    protected G3MauiCoreException(
        string message,
        Exception innerException)
        : base(
            message,
            innerException)
    {
    }
}