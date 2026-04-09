using System;
using System.Threading;
using System.Threading.Tasks;
using G3.Maui.Core.ViewModels;
using Xunit;

namespace G3.Maui.Core.Tests.ViewModels;

public sealed class BaseViewModelTests : IDisposable
{
    private readonly TestableViewModel _sut = new();

    [Fact]
    public void IsBusy_DefaultValue_IsFalse()
    {
        Assert.False(_sut.IsBusy);
    }

    [Fact]
    public void IsNotBusy_DefaultValue_IsTrue()
    {
        Assert.True(_sut.IsNotBusy);
    }

    [Fact]
    public void IsNotBusy_WhenBusy_IsFalse()
    {
        _sut.IsBusy = true;

        Assert.False(_sut.IsNotBusy);
    }

    [Fact]
    public void Title_DefaultValue_IsNull()
    {
        Assert.Null(_sut.Title);
    }

    [Fact]
    public void LoadingMessage_DefaultValue_IsLoading()
    {
        Assert.Equal("Loading...", _sut.LoadingMessage);
    }

    [Fact]
    public async Task SafeExecuteAsync_SetsBusyDuringOperation()
    {
        var busyDuringOperation = false;

        await _sut.ExecuteSafe(async () =>
        {
            busyDuringOperation = _sut.IsBusy;
            await Task.CompletedTask;
        });

        Assert.True(busyDuringOperation);
    }

    [Fact]
    public async Task SafeExecuteAsync_ClearsBusyAfterOperation()
    {
        await _sut.ExecuteSafe(async () => await Task.CompletedTask);

        Assert.False(_sut.IsBusy);
    }

    [Fact]
    public async Task SafeExecuteAsync_ClearsBusyAfterException()
    {
        await _sut.ExecuteSafe(
            async () =>
            {
                await Task.CompletedTask;
                throw new InvalidOperationException("test");
            },
            onError: _ => { });

        Assert.False(_sut.IsBusy);
    }

    [Fact]
    public async Task SafeExecuteAsync_SetsLoadingMessage_WhenProvided()
    {
        await _sut.ExecuteSafe(
            async () => await Task.CompletedTask,
            loadingMessage: "Saving...");

        Assert.Equal("Saving...", _sut.LoadingMessage);
    }

    [Fact]
    public async Task SafeExecuteAsync_DoesNotChangeLoadingMessage_WhenNotProvided()
    {
        await _sut.ExecuteSafe(async () => await Task.CompletedTask);

        Assert.Equal("Loading...", _sut.LoadingMessage);
    }

    [Fact]
    public async Task SafeExecuteAsync_CallsOnError_WhenExceptionThrown()
    {
        Exception? caughtException = null;

        await _sut.ExecuteSafe(
            async () =>
            {
                await Task.CompletedTask;
                throw new InvalidOperationException("boom");
            },
            onError: ex => caughtException = ex);

        Assert.IsType<InvalidOperationException>(caughtException);
        Assert.Equal("boom", caughtException!.Message);
    }

    [Fact]
    public async Task SafeExecuteAsync_SwallowsException_WhenOnErrorIsNull()
    {
        var exception = await Record.ExceptionAsync(
            () => _sut.ExecuteSafe(
                async () =>
                {
                    await Task.CompletedTask;
                    throw new InvalidOperationException("swallow me");
                }));

        Assert.Null(exception);
    }

    [Fact]
    public void PageCancellationToken_IsNotCancelledByDefault()
    {
        var token = _sut.PageCancellationToken;

        Assert.False(token.IsCancellationRequested);
    }

    [Fact]
    public void PageCancellationToken_ReturnsSameTokenOnMultipleCalls()
    {
        var token1 = _sut.PageCancellationToken;
        var token2 = _sut.PageCancellationToken;

        Assert.Equal(token1, token2);
    }

    [Fact]
    public void CancelPageOperations_CancelsTheToken()
    {
        var token = _sut.PageCancellationToken;

        _sut.CancelPageOperations();

        Assert.True(token.IsCancellationRequested);
    }

    [Fact]
    public void ResetPageCancellationToken_CreatesNewToken()
    {
        var originalToken = _sut.PageCancellationToken;

        _sut.ResetPageCancellationToken();
        var newToken = _sut.PageCancellationToken;

        Assert.NotEqual(originalToken, newToken);
        Assert.False(newToken.IsCancellationRequested);
    }

    [Fact]
    public void ResetPageCancellationToken_CancelsPreviousToken()
    {
        var originalToken = _sut.PageCancellationToken;

        _sut.ResetPageCancellationToken();

        Assert.True(originalToken.IsCancellationRequested);
    }

    [Fact]
    public void Dispose_CancelsActiveToken()
    {
        var token = _sut.PageCancellationToken;

        _sut.Dispose();

        Assert.True(token.IsCancellationRequested);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        _sut.Dispose();
        var exception = Record.Exception(() => _sut.Dispose());

        Assert.Null(exception);
    }

    public void Dispose() => _sut.Dispose();

    private sealed class TestableViewModel : BaseViewModel
    {
        public Task ExecuteSafe(
            Func<Task> operation,
            string? loadingMessage = null,
            Action<Exception>? onError = null)
            => SafeExecuteAsync(
                operation,
                loadingMessage,
                onError);
    }
}
