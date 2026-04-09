using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace G3.Maui.Core.ViewModels;

/// <summary>
/// Base view model providing busy state, loading message, page-scoped cancellation,
/// and a <see cref="SafeExecuteAsync"/> helper that eliminates IsBusy boilerplate.
/// </summary>
public partial class BaseViewModel : ObservableObject, IDisposable
{
    private CancellationTokenSource? _pageCancellationTokenSource;
    private bool _disposed;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private string _loadingMessage = "Loading...";

    /// <summary>True when <see cref="IsBusy"/> is false.</summary>
    public bool IsNotBusy => !IsBusy;

    /// <summary>
    /// Cancellation token that is cancelled when the page navigates away.
    /// Use for GET/read operations that should cancel on navigation.
    /// </summary>
    public CancellationToken PageCancellationToken
    {
        get
        {
            _pageCancellationTokenSource ??= new CancellationTokenSource();
            return _pageCancellationTokenSource.Token;
        }
    }

    /// <summary>
    /// Sets <see cref="IsBusy"/> to true, runs <paramref name="operation"/>, then resets it in
    /// a finally block. Catches exceptions and forwards them to <paramref name="onError"/>;
    /// if <paramref name="onError"/> is null, exceptions are swallowed.
    /// Optionally updates <see cref="LoadingMessage"/> before the operation starts.
    /// </summary>
    /// <param name="operation">The async work to execute.</param>
    /// <param name="loadingMessage">If provided, sets <see cref="LoadingMessage"/> before starting.</param>
    /// <param name="onError">Optional handler for any exception thrown by <paramref name="operation"/>.</param>
    protected async Task SafeExecuteAsync(
        Func<Task> operation,
        string? loadingMessage = null,
        Action<Exception>? onError = null)
    {
        IsBusy = true;
        if (loadingMessage != null)
        {
            LoadingMessage = loadingMessage;
        }
        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Cancels all ongoing operations tied to the page lifecycle.
    /// Called automatically by <see cref="G3.Maui.Core.Views.BasePage"/> on disappear.
    /// </summary>
    public virtual void CancelPageOperations()
    {
        _pageCancellationTokenSource?.Cancel();
        _pageCancellationTokenSource?.Dispose();
        _pageCancellationTokenSource = null;
    }

    /// <summary>
    /// Resets the page cancellation token for a fresh page session.
    /// Called automatically by <see cref="G3.Maui.Core.Views.BasePage"/> on appear.
    /// </summary>
    public virtual void ResetPageCancellationToken()
    {
        _pageCancellationTokenSource?.Cancel();
        _pageCancellationTokenSource?.Dispose();
        _pageCancellationTokenSource = new CancellationTokenSource();
    }

    /// <inheritdoc />
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _pageCancellationTokenSource?.Cancel();
                _pageCancellationTokenSource?.Dispose();
                _pageCancellationTokenSource = null;
            }

            _disposed = true;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
