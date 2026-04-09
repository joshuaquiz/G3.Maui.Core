using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Behaviors;

/// <summary>
/// Executes a command when a page appears.
/// Attach to a page's Behaviors collection and bind <see cref="Command"/> to a view-model command.
/// </summary>
public class PageAppearingBehavior : Behavior<Page>
{
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(PageAppearingBehavior));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(PageAppearingBehavior));

    /// <summary>The command to execute when the page appears.</summary>
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

    protected override void OnAttachedTo(Page bindable)
    {
        base.OnAttachedTo(bindable);
        _page = bindable;
        _page.Appearing += OnAppearing;
    }

    protected override void OnDetachingFrom(Page bindable)
    {
        base.OnDetachingFrom(bindable);
        if (_page != null)
        {
            _page.Appearing -= OnAppearing;
        }

        _page = null;
    }

    private void OnAppearing(
        object? sender,
        EventArgs e)
    {
        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }
}
