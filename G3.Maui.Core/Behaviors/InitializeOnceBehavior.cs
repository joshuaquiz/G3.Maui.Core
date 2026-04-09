using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Behaviors;

/// <summary>
/// Executes a command only once when the page first appears.
/// Set <see cref="ResetOnDisappearing"/> to true to re-run on the next appearance
/// (e.g., after the user navigates away and returns).
/// </summary>
public class InitializeOnceBehavior : Behavior<Page>
{
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(InitializeOnceBehavior));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(InitializeOnceBehavior));

    public static readonly BindableProperty ResetOnDisappearingProperty =
        BindableProperty.Create(nameof(ResetOnDisappearing), typeof(bool), typeof(InitializeOnceBehavior), false);

    /// <summary>The command to execute on first appearance.</summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>Optional parameter passed to <see cref="Command"/> on execution.</summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// When true, the behavior resets when the page disappears so the command
    /// will fire again on the next appearance. Defaults to false.
    /// </summary>
    public bool ResetOnDisappearing
    {
        get => (bool)GetValue(ResetOnDisappearingProperty);
        set => SetValue(ResetOnDisappearingProperty, value);
    }

    private Page? _page;
    private bool _initialized;

    protected override void OnAttachedTo(Page bindable)
    {
        base.OnAttachedTo(bindable);
        _page = bindable;
        _page.Appearing += OnAppearing;
        if (ResetOnDisappearing)
        {
            _page.Disappearing += OnDisappearing;
        }
    }

    protected override void OnDetachingFrom(Page bindable)
    {
        base.OnDetachingFrom(bindable);
        if (_page != null)
        {
            _page.Appearing -= OnAppearing;
            _page.Disappearing -= OnDisappearing;
        }

        _page = null;
    }

    private void OnAppearing(
        object? sender,
        EventArgs e)
    {
        if (!_initialized && Command?.CanExecute(CommandParameter) == true)
        {
            _initialized = true;
            Command.Execute(CommandParameter);
        }
    }

    private void OnDisappearing(
        object? sender,
        EventArgs e)
    {
        _initialized = false;
    }
}
