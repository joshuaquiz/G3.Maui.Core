using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Components.EmptyState;

/// <summary>
/// A reusable empty-state view for displaying when a list or page has no content.
/// Shows an icon, title, message, and an optional action button (e.g., "Retry").
/// Set <see cref="ShowActionButton"/> to true and bind <see cref="ActionCommand"/> to enable the button.
/// </summary>
public partial class EmptyStateView : ContentView
{
    /// <summary>Bindable property for <see cref="IconSource"/>.</summary>
    public static readonly BindableProperty IconSourceProperty =
        BindableProperty.Create(nameof(IconSource), typeof(string), typeof(EmptyStateView), "search");

    /// <summary>Bindable property for <see cref="Title"/>.</summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(EmptyStateView), "No Items Found");

    /// <summary>Bindable property for <see cref="Message"/>.</summary>
    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(EmptyStateView), "There are no items to display.");

    /// <summary>Bindable property for <see cref="ActionButtonText"/>.</summary>
    public static readonly BindableProperty ActionButtonTextProperty =
        BindableProperty.Create(nameof(ActionButtonText), typeof(string), typeof(EmptyStateView), "Retry");

    /// <summary>Bindable property for <see cref="ActionCommand"/>.</summary>
    public static readonly BindableProperty ActionCommandProperty =
        BindableProperty.Create(nameof(ActionCommand), typeof(ICommand), typeof(EmptyStateView), null);

    /// <summary>Bindable property for <see cref="ShowActionButton"/>.</summary>
    public static readonly BindableProperty ShowActionButtonProperty =
        BindableProperty.Create(nameof(ShowActionButton), typeof(bool), typeof(EmptyStateView), false);

    /// <summary>Icon resource name to display. Defaults to "search".</summary>
    public string IconSource
    {
        get => (string)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>Heading text shown below the icon. Defaults to "No Items Found".</summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Body text shown below the title. Defaults to "There are no items to display."</summary>
    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>Label for the optional action button. Defaults to "Retry".</summary>
    public string ActionButtonText
    {
        get => (string)GetValue(ActionButtonTextProperty);
        set => SetValue(ActionButtonTextProperty, value);
    }

    /// <summary>Command executed when the action button is tapped.</summary>
    public ICommand? ActionCommand
    {
        get => (ICommand?)GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    /// <summary>When true, the action button is visible. Defaults to false.</summary>
    public bool ShowActionButton
    {
        get => (bool)GetValue(ShowActionButtonProperty);
        set => SetValue(ShowActionButtonProperty, value);
    }

    /// <summary>Initializes a new instance of <see cref="EmptyStateView"/>.</summary>
    public EmptyStateView()
    {
        InitializeComponent();
    }
}
