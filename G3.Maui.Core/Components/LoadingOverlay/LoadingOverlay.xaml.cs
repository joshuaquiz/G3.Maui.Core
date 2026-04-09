using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Components.LoadingOverlay;

/// <summary>
/// Full-screen loading overlay that blocks interaction while an operation is in progress.
/// Set <see cref="IsLoading"/> to true/false to show/hide, or call <see cref="Show"/>/<see cref="Hide"/>.
/// Optionally bind <see cref="CancellationCommand"/> to allow the user to abort the operation.
/// The cancel button appears after <see cref="CancellationDelay"/> (default 5 seconds) so it does
/// not distract the user during fast operations.
/// </summary>
public partial class LoadingOverlay : Grid
{
    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(LoadingOverlay), false,
            propertyChanged: OnIsLoadingChanged);

    public static readonly BindableProperty LoadingMessageProperty =
        BindableProperty.Create(nameof(LoadingMessage), typeof(string), typeof(LoadingOverlay), "Loading...");

    public static readonly BindableProperty CancellationCommandProperty =
        BindableProperty.Create(nameof(CancellationCommand), typeof(ICommand), typeof(LoadingOverlay), null);

    public static readonly BindableProperty CancellationDelayProperty =
        BindableProperty.Create(nameof(CancellationDelay), typeof(TimeSpan), typeof(LoadingOverlay), TimeSpan.FromSeconds(5));

    public static readonly BindableProperty CancellationButtonTextProperty =
        BindableProperty.Create(nameof(CancellationButtonText), typeof(string), typeof(LoadingOverlay), "Cancel");

    private CancellationTokenSource? _delayCts;

    /// <summary>Shows or hides the overlay. Triggers the cancel button timer when set to true.</summary>
    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>Message displayed below the spinner. Defaults to "Loading...".</summary>
    public string LoadingMessage
    {
        get => (string)GetValue(LoadingMessageProperty);
        set => SetValue(LoadingMessageProperty, value);
    }

    /// <summary>
    /// Command to execute when the user taps the cancel button.
    /// If null (the default), the cancel button is never shown.
    /// The command is responsible for stopping the operation and setting <see cref="IsLoading"/> to false.
    /// </summary>
    public ICommand? CancellationCommand
    {
        get => (ICommand?)GetValue(CancellationCommandProperty);
        set => SetValue(CancellationCommandProperty, value);
    }

    /// <summary>
    /// How long after loading starts before the cancel button appears. Defaults to 5 seconds.
    /// Set to <see cref="TimeSpan.Zero"/> to show the button immediately.
    /// </summary>
    public TimeSpan CancellationDelay
    {
        get => (TimeSpan)GetValue(CancellationDelayProperty);
        set => SetValue(CancellationDelayProperty, value);
    }

    /// <summary>Label shown on the cancel button. Defaults to "Cancel".</summary>
    public string CancellationButtonText
    {
        get => (string)GetValue(CancellationButtonTextProperty);
        set => SetValue(CancellationButtonTextProperty, value);
    }

    public LoadingOverlay()
    {
        InitializeComponent();
    }

    private static void OnIsLoadingChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        if (bindable is not LoadingOverlay overlay)
        {
            return;
        }

        var loading = (bool)newValue;
        overlay.IsVisible = loading;
        overlay.LoadingIndicator.IsRunning = loading;
        if (loading)
        {
            overlay.StartCancelButtonTimer();
        }
        else
        {
            overlay.StopCancelButtonTimer();
        }
    }

    private void StartCancelButtonTimer()
    {
        StopCancelButtonTimer();
        if (CancellationCommand is null)
        {
            return;
        }

        _delayCts = new CancellationTokenSource();
        ShowCancelButtonAfterDelay(_delayCts.Token);
    }

    private async void ShowCancelButtonAfterDelay(CancellationToken token)
    {
        try
        {
            await Task.Delay(CancellationDelay, token);
            CancelButton.IsVisible = true;
        }
        catch (OperationCanceledException) { }
    }

    private void StopCancelButtonTimer()
    {
        _delayCts?.Cancel();
        _delayCts?.Dispose();
        _delayCts = null;
        CancelButton.IsVisible = false;
    }

    /// <summary>Shows the overlay and optionally updates the loading message.</summary>
    public void Show(string? message = null)
    {
        if (message != null)
        {
            LoadingMessage = message;
        }

        IsLoading = true;
    }

    /// <summary>Hides the overlay and cancels any pending cancel-button timer.</summary>
    public void Hide() => IsLoading = false;
}
