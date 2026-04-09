using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Behaviors;

/// <summary>
/// Executes a command when a page disappears.
/// Attach to a page's Behaviors collection and bind <see cref="Command"/> to a view-model command.
/// </summary>
public class PageDisappearingBehavior : Behavior<Page>
{
    /// <summary>Bindable property for <see cref="Command"/>.</summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(PageDisappearingBehavior));

    /// <summary>Bindable property for <see cref="CommandParameter"/>.</summary>
    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(PageDisappearingBehavior));

    /// <summary>The command to execute when the page disappears.</summary>
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

    private Page? _page;

    /// <inheritdoc />
    protected override void OnAttachedTo(Page bindable)
    {
        base.OnAttachedTo(bindable);
        _page = bindable;
        _page.Disappearing += OnDisappearing;
    }

    /// <inheritdoc />
    protected override void OnDetachingFrom(Page bindable)
    {
        base.OnDetachingFrom(bindable);
        if (_page != null)
        {
            _page.Disappearing -= OnDisappearing;
        }

        _page = null;
    }

    private void OnDisappearing(
        object? sender,
        EventArgs e)
    {
        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }
}
