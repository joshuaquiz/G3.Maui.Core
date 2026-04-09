using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Behaviors;

/// <summary>
/// Executes a command when a page is loaded (added to the visual tree).
/// Set <see cref="ExecuteOnce"/> to false to re-execute on every load.
/// </summary>
public class PageLoadedBehavior : Behavior<Page>
{
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(PageLoadedBehavior));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(PageLoadedBehavior));

    public static readonly BindableProperty ExecuteOnceProperty =
        BindableProperty.Create(nameof(ExecuteOnce), typeof(bool), typeof(PageLoadedBehavior), true);

    /// <summary>The command to execute when the page loads.</summary>
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

    /// <summary>If true (default), the command executes only once and the handler is unsubscribed.</summary>
    public bool ExecuteOnce
    {
        get => (bool)GetValue(ExecuteOnceProperty);
        set => SetValue(ExecuteOnceProperty, value);
    }

    private Page? _page;
    private bool _executed;

    protected override void OnAttachedTo(Page bindable)
    {
        base.OnAttachedTo(bindable);
        _page = bindable;
        _page.Loaded += OnLoaded;
    }

    protected override void OnDetachingFrom(Page bindable)
    {
        base.OnDetachingFrom(bindable);
        if (_page != null)
        {
            _page.Loaded -= OnLoaded;
        }

        _page = null;
    }

    private void OnLoaded(
        object? sender,
        EventArgs e)
    {
        if (ExecuteOnce && _executed)
        {
            return;
        }
        if (Command?.CanExecute(CommandParameter) == true)
        {
            _executed = true;
            Command.Execute(CommandParameter);
        }
        if (ExecuteOnce && _page != null)
        {
            _page.Loaded -= OnLoaded;
        }
    }
}
