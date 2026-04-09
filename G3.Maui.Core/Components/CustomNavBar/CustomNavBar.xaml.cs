using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using G3.Maui.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;

namespace G3.Maui.Core.Components.CustomNavBar;

/// <summary>
/// A custom navigation bar that replaces the Shell nav bar.
/// Supports a title, an optional back button, and a collection of <see cref="NavBarCommand"/> icon buttons.
/// Commands that do not fit within the available width are collapsed into an overflow action sheet.
/// Bind <see cref="Commands"/> to an <see cref="ObservableCollection{NavBarCommand}"/> in your view model.
/// Colors automatically adapt to the device theme; override via <see cref="BackgroundColor"/> and <see cref="IconTintColor"/>.
/// </summary>
public partial class CustomNavBar : ContentView
{
    private const double TITLE_MAX_WIDTH_PERCENTAGE = 0.75;
    private const double ICON_SIZE = 18.0;
    private const double ICON_SPACING = 16.0;
    private const double BACK_BUTTON_AREA_WIDTH = 32.0;
    private const double NAVBAR_HORIZONTAL_PADDING = 40.0;

    private readonly ILogger<CustomNavBar>? _logger;
    private List<NavBarCommand> _overflowCommands = [];

    #region Bindable Properties

    /// <summary>Bindable property for <see cref="Title"/>.</summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(CustomNavBar), string.Empty,
            propertyChanged: OnTitleChanged);

    /// <summary>Bindable property for <see cref="ShowBackButton"/>.</summary>
    public static readonly BindableProperty ShowBackButtonProperty =
        BindableProperty.Create(nameof(ShowBackButton), typeof(bool), typeof(CustomNavBar), false);

    /// <summary>Bindable property for <see cref="BackCommand"/>.</summary>
    public static readonly BindableProperty BackCommandProperty =
        BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(CustomNavBar), null);

    /// <summary>Bindable property for <see cref="Commands"/>.</summary>
    public static readonly BindableProperty CommandsProperty =
        BindableProperty.Create(nameof(Commands), typeof(ObservableCollection<NavBarCommand>), typeof(CustomNavBar),
            null, propertyChanged: OnCommandsChanged);

    /// <summary>Bindable property for <see cref="Height"/>.</summary>
    public static new readonly BindableProperty HeightProperty =
        BindableProperty.Create(nameof(Height), typeof(double), typeof(CustomNavBar), 56.0);

    /// <summary>Bindable property for <see cref="BackgroundColor"/>.</summary>
    public static new readonly BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(CustomNavBar), null,
            propertyChanged: OnBackgroundColorChanged);

    /// <summary>Bindable property for <see cref="IconTintColor"/>.</summary>
    public static readonly BindableProperty IconTintColorProperty =
        BindableProperty.Create(nameof(IconTintColor), typeof(Color), typeof(CustomNavBar), null,
            propertyChanged: OnIconTintColorChanged);

    /// <summary>Bindable property for <see cref="HasTitle"/>.</summary>
    public static readonly BindableProperty HasTitleProperty =
        BindableProperty.Create(nameof(HasTitle), typeof(bool), typeof(CustomNavBar), false);

    /// <summary>Bindable property for <see cref="TitleIsEditable"/>.</summary>
    public static readonly BindableProperty TitleIsEditableProperty =
        BindableProperty.Create(nameof(TitleIsEditable), typeof(bool), typeof(CustomNavBar), false);

    #endregion

    #region Properties

    /// <summary>The title displayed in the center of the nav bar.</summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>When true, a back button is shown on the leading side.</summary>
    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    /// <summary>Command executed when the back button is tapped.</summary>
    public ICommand? BackCommand
    {
        get => (ICommand?)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    /// <summary>
    /// The icon buttons to show on the trailing side of the nav bar.
    /// Commands that exceed the available width are collapsed into an overflow menu.
    /// </summary>
    public ObservableCollection<NavBarCommand>? Commands
    {
        get => (ObservableCollection<NavBarCommand>?)GetValue(CommandsProperty);
        set => SetValue(CommandsProperty, value);
    }

    /// <summary>Nav bar height in device-independent units. Defaults to 56.</summary>
    public new double Height
    {
        get => (double)GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }

    /// <summary>Nav bar background color. Defaults to a theme-appropriate color (purple on light, dark gray on dark).</summary>
    public new Color? BackgroundColor
    {
        get => (Color?)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    /// <summary>Tint color applied to all icon images. Defaults to white.</summary>
    public Color? IconTintColor
    {
        get => (Color?)GetValue(IconTintColorProperty);
        set => SetValue(IconTintColorProperty, value);
    }

    /// <summary>True when <see cref="Title"/> is not null or whitespace. Updated automatically.</summary>
    public bool HasTitle
    {
        get => (bool)GetValue(HasTitleProperty);
        private set => SetValue(HasTitleProperty, value);
    }

    /// <summary>When true, the title renders as an editable field. Defaults to false.</summary>
    public bool TitleIsEditable
    {
        get => (bool)GetValue(TitleIsEditableProperty);
        set => SetValue(TitleIsEditableProperty, value);
    }

    #endregion

    /// <summary>Initializes a new instance of <see cref="CustomNavBar"/>.</summary>
    public CustomNavBar()
    {
        InitializeComponent();
        Commands = [];
        SetDefaultColors();

        _logger = Application.Current?.Handler?.MauiContext?.Services
            .GetService(typeof(ILogger<CustomNavBar>)) as ILogger<CustomNavBar>;
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeChanged += OnThemeChanged;
        }
    }

    private void OnThemeChanged(
        object? sender,
        AppThemeChangedEventArgs e)
    {
        BackgroundColor = null;
        IconTintColor = null;
        SetDefaultColors();
    }

    private static void OnTitleChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is CustomNavBar nav)
        {
            nav.HasTitle = !string.IsNullOrWhiteSpace(n as string);
        }
    }

    private static void OnCommandsChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is not CustomNavBar nav)
        {
            return;
        }
        if (o is ObservableCollection<NavBarCommand> old)
        {
            old.CollectionChanged -= nav.OnCommandsCollectionChanged;
        }
        if (n is ObservableCollection<NavBarCommand> @new)
        {
            @new.CollectionChanged += nav.OnCommandsCollectionChanged;
        }

        nav.RebuildCommandIcons();
    }

    private void OnCommandsCollectionChanged(
        object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        => RebuildCommandIcons();

    private static void OnBackgroundColorChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is CustomNavBar nav && n == null)
        {
            nav.SetDefaultColors();
        }
    }

    private static void OnIconTintColorChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is CustomNavBar nav && n == null)
        {
            nav.SetDefaultColors();
        }
    }

    private void SetDefaultColors()
    {
        BackgroundColor ??= Application.Current?.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#1C1C1E")
            : Color.FromArgb("#512BD4");
        IconTintColor ??= Colors.White;
    }

    private void RebuildCommandIcons()
    {
        CommandsContainer.Clear();
        _overflowCommands.Clear();
        if (Commands == null || Commands.Count == 0)
        {
            return;
        }

        var available = CalculateAvailableCommandSpace();
        var needed = Commands.Count * (ICON_SIZE + ICON_SPACING) - ICON_SPACING;
        if (needed > available)
        {
            _overflowCommands = Commands.ToList();
            CommandsContainer.Add(CreateOverflowButton());
        }
        else
        {
            foreach (var cmd in Commands)
            {
                CommandsContainer.Add(CreateCommandButton(cmd));
            }
        }
    }

    private double CalculateAvailableCommandSpace()
    {
        var totalWidth = Width > 0 ? Width : DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        var available = totalWidth - NAVBAR_HORIZONTAL_PADDING;
        if (ShowBackButton)
        {
            available -= BACK_BUTTON_AREA_WIDTH;
        }

        available -= ICON_SPACING;
        if (HasTitle && !string.IsNullOrWhiteSpace(Title))
        {
            var titleWidth = Math.Min(Title.Length * 20.0 * 0.6, available * TITLE_MAX_WIDTH_PERCENTAGE);
            available -= titleWidth + ICON_SPACING;
        }

        return Math.Max(0, available);
    }

    private ImageButton CreateCommandButton(NavBarCommand cmd)
    {
        var btn = new ImageButton
        {
            Source = cmd.IconSource,
            AutomationId = $"NavBarCommand_{cmd.DisplayName}",
            WidthRequest = ICON_SIZE + 18,
            HeightRequest = ICON_SIZE + 18,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Transparent,
            Aspect = Aspect.AspectFit,
            Padding = 8,
            Margin = 0,
            Command = cmd.Command,
            CommandParameter = cmd.CommandParameter
        };
        btn.Behaviors.Add(new CommunityToolkit.Maui.Behaviors.IconTintColorBehavior { TintColor = IconTintColor ?? Colors.White });
        return btn;
    }

    private ImageButton CreateOverflowButton()
    {
        var btn = new ImageButton
        {
            Source = "more_vert",
            AutomationId = "NavBarOverflow",
            WidthRequest = ICON_SIZE + 18,
            HeightRequest = ICON_SIZE + 18,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Transparent,
            Aspect = Aspect.AspectFit,
            Padding = 8,
            Margin = 0
        };
        btn.Clicked += OnOverflowClicked;
        btn.Behaviors.Add(new CommunityToolkit.Maui.Behaviors.IconTintColorBehavior { TintColor = IconTintColor ?? Colors.White });
        return btn;
    }

    private async void OnOverflowClicked(
        object? sender,
        EventArgs e)
    {
        if (_overflowCommands.Count == 0)
        {
            return;
        }

        var items = _overflowCommands.Select(c => c.DisplayName).ToArray();
        var result = await Shell.Current.DisplayActionSheetAsync("Actions", "Cancel", null, items);
        if (result == null || result == "Cancel")
        {
            return;
        }

        var selected = _overflowCommands.FirstOrDefault(c => c.DisplayName == result);
        if (selected?.Command?.CanExecute(selected.CommandParameter) == true)
        {
            selected.Command.Execute(selected.CommandParameter);
        }
    }
}
