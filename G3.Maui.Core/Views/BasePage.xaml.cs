using System.Collections.ObjectModel;
using System.Windows.Input;
using G3.Maui.Core.Components.LoadingOverlay;
using G3.Maui.Core.Models;
using G3.Maui.Core.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace G3.Maui.Core.Views;

/// <summary>
/// Base page for the application. Hides the Shell nav bar and tab bar, wires the loading overlay
/// to the view model's IsBusy, and provides a custom nav bar via <see cref="ShowTopNavBar"/>.
/// Override <see cref="OnPageReady"/> to run logic after the first layout pass.
/// Set <see cref="UseSafeArea"/> to true on iOS to avoid content hiding behind the notch.
/// </summary>
public partial class BasePage : ContentPage
{
    private bool _pageReadyFired;

    #region Bindable Properties

    /// <summary>Bindable property for <see cref="ShowTopNavBar"/>.</summary>
    public static readonly BindableProperty ShowTopNavBarProperty =
        BindableProperty.Create(nameof(ShowTopNavBar), typeof(bool), typeof(BasePage), true);

    /// <summary>Bindable property for <see cref="ShowBottomNavBar"/>.</summary>
    public static readonly BindableProperty ShowBottomNavBarProperty =
        BindableProperty.Create(nameof(ShowBottomNavBar), typeof(bool), typeof(BasePage), false);

    /// <summary>Bindable property for <see cref="NavBarTitle"/>.</summary>
    public static readonly BindableProperty NavBarTitleProperty =
        BindableProperty.Create(nameof(NavBarTitle), typeof(string), typeof(BasePage), string.Empty);

    /// <summary>Bindable property for <see cref="ShowNavBarBackButton"/>.</summary>
    public static readonly BindableProperty ShowNavBarBackButtonProperty =
        BindableProperty.Create(nameof(ShowNavBarBackButton), typeof(bool), typeof(BasePage), true);

    /// <summary>Bindable property for <see cref="NavBarBackCommand"/>.</summary>
    public static readonly BindableProperty NavBarBackCommandProperty =
        BindableProperty.Create(nameof(NavBarBackCommand), typeof(ICommand), typeof(BasePage), null);

    /// <summary>Bindable property for <see cref="NavBarCommands"/>.</summary>
    public static readonly BindableProperty NavBarCommandsProperty =
        BindableProperty.Create(nameof(NavBarCommands), typeof(ObservableCollection<NavBarCommand>), typeof(BasePage), null);

    /// <summary>Bindable property for <see cref="NavBarBackgroundColor"/>.</summary>
    public static readonly BindableProperty NavBarBackgroundColorProperty =
        BindableProperty.Create(nameof(NavBarBackgroundColor), typeof(Color), typeof(BasePage), null);

    /// <summary>Bindable property for <see cref="NavBarIconTintColor"/>.</summary>
    public static readonly BindableProperty NavBarIconTintColorProperty =
        BindableProperty.Create(nameof(NavBarIconTintColor), typeof(Color), typeof(BasePage), null);

    /// <summary>
    /// When true, the ad slot row is visible. Set <see cref="AdContent"/> to supply the actual ad view.
    /// </summary>
    public static readonly BindableProperty ShowAdPlaceholderProperty =
        BindableProperty.Create(nameof(ShowAdPlaceholder), typeof(bool), typeof(BasePage), false);

    /// <summary>Bindable property for <see cref="NavBarTitleIsEditable"/>.</summary>
    public static readonly BindableProperty NavBarTitleIsEditableProperty =
        BindableProperty.Create(nameof(NavBarTitleIsEditable), typeof(bool), typeof(BasePage), false);

    /// <summary>
    /// The view to display in the ad slot (e.g., a banner ad control from your ad SDK).
    /// Only shown when <see cref="ShowAdPlaceholder"/> is true.
    /// </summary>
    public static readonly BindableProperty AdContentProperty =
        BindableProperty.Create(nameof(AdContent), typeof(View), typeof(BasePage), null);

    /// <summary>
    /// The view to display in the bottom bar slot (e.g., a custom tab bar).
    /// Only shown when <see cref="ShowBottomNavBar"/> is true.
    /// </summary>
    public static readonly BindableProperty BottomBarContentProperty =
        BindableProperty.Create(nameof(BottomBarContent), typeof(View), typeof(BasePage), null);

    /// <summary>
    /// When true, applies iOS safe area insets so page content does not extend behind the notch
    /// or home indicator. Has no effect on Android or other platforms. Defaults to false.
    /// </summary>
    public static readonly BindableProperty UseSafeAreaProperty =
        BindableProperty.Create(nameof(UseSafeArea), typeof(bool), typeof(BasePage), false,
            propertyChanged: OnUseSafeAreaChanged);

    #endregion

    #region Properties

    /// <summary>When true, the custom top nav bar is visible. Defaults to true.</summary>
    public bool ShowTopNavBar
    {
        get => (bool)GetValue(ShowTopNavBarProperty);
        set => SetValue(ShowTopNavBarProperty, value);
    }

    /// <summary>When true, the bottom bar slot is visible. Defaults to false.</summary>
    public bool ShowBottomNavBar
    {
        get => (bool)GetValue(ShowBottomNavBarProperty);
        set => SetValue(ShowBottomNavBarProperty, value);
    }

    /// <summary>Title text displayed in the custom nav bar.</summary>
    public string NavBarTitle
    {
        get => (string)GetValue(NavBarTitleProperty);
        set => SetValue(NavBarTitleProperty, value);
    }

    /// <summary>When true, a back button is shown in the nav bar. Defaults to true.</summary>
    public bool ShowNavBarBackButton
    {
        get => (bool)GetValue(ShowNavBarBackButtonProperty);
        set => SetValue(ShowNavBarBackButtonProperty, value);
    }

    /// <summary>Command executed when the nav bar back button is tapped.</summary>
    public ICommand? NavBarBackCommand
    {
        get => (ICommand?)GetValue(NavBarBackCommandProperty);
        set => SetValue(NavBarBackCommandProperty, value);
    }

    /// <summary>Icon button commands displayed on the trailing side of the nav bar.</summary>
    public ObservableCollection<NavBarCommand>? NavBarCommands
    {
        get => (ObservableCollection<NavBarCommand>?)GetValue(NavBarCommandsProperty);
        set => SetValue(NavBarCommandsProperty, value);
    }

    /// <summary>Background color of the nav bar. Defaults to a theme-appropriate color.</summary>
    public Color? NavBarBackgroundColor
    {
        get => (Color?)GetValue(NavBarBackgroundColorProperty);
        set => SetValue(NavBarBackgroundColorProperty, value);
    }

    /// <summary>Tint color applied to nav bar icons. Defaults to white.</summary>
    public Color? NavBarIconTintColor
    {
        get => (Color?)GetValue(NavBarIconTintColorProperty);
        set => SetValue(NavBarIconTintColorProperty, value);
    }

    /// <summary>When true, the ad slot row is visible. Set <see cref="AdContent"/> to supply the actual ad view.</summary>
    public bool ShowAdPlaceholder
    {
        get => (bool)GetValue(ShowAdPlaceholderProperty);
        set => SetValue(ShowAdPlaceholderProperty, value);
    }

    /// <summary>When true, the nav bar title renders as an editable field. Defaults to false.</summary>
    public bool NavBarTitleIsEditable
    {
        get => (bool)GetValue(NavBarTitleIsEditableProperty);
        set => SetValue(NavBarTitleIsEditableProperty, value);
    }

    /// <summary>The view displayed in the ad slot. Only visible when <see cref="ShowAdPlaceholder"/> is true.</summary>
    public View? AdContent
    {
        get => (View?)GetValue(AdContentProperty);
        set => SetValue(AdContentProperty, value);
    }

    /// <summary>The view displayed in the bottom bar slot. Only visible when <see cref="ShowBottomNavBar"/> is true.</summary>
    public View? BottomBarContent
    {
        get => (View?)GetValue(BottomBarContentProperty);
        set => SetValue(BottomBarContentProperty, value);
    }

    /// <summary>When true, applies iOS safe area insets to avoid content hiding behind the notch. Defaults to false.</summary>
    public bool UseSafeArea
    {
        get => (bool)GetValue(UseSafeAreaProperty);
        set => SetValue(UseSafeAreaProperty, value);
    }

    #endregion

    /// <summary>Initializes a new instance of <see cref="BasePage"/>.</summary>
    public BasePage()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyShellChromeVisibility();
        (BindingContext as BaseViewModel)?.ResetPageCancellationToken();
    }

    /// <inheritdoc />
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        ApplyShellChromeVisibility();
    }

    /// <inheritdoc />
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        (BindingContext as BaseViewModel)?.CancelPageOperations();
    }

    /// <inheritdoc />
    protected override bool OnBackButtonPressed()
    {
        (BindingContext as BaseViewModel)?.CancelPageOperations();
        if (NavBarBackCommand?.CanExecute(null) == true)
        {
            NavBarBackCommand.Execute(null);
            return true;
        }

        return base.OnBackButtonPressed();
    }

    /// <inheritdoc />
    protected override void OnSizeAllocated(
        double width,
        double height)
    {
        base.OnSizeAllocated(width, height);
        if (!_pageReadyFired && width > 0 && height > 0)
        {
            _pageReadyFired = true;
            OnPageReady();
        }
    }

    /// <summary>
    /// Called once after the first successful layout pass (non-zero width and height).
    /// Override to trigger initial data loads or animations that require final dimensions.
    /// Prefer this over <c>OnAppearing</c> when the logic depends on the
    /// page being fully laid out.
    /// </summary>
    protected virtual void OnPageReady() { }

    private static void OnUseSafeAreaChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is BasePage page && n is bool useSafeArea)
        {
            page.SafeAreaEdges = useSafeArea ? Microsoft.Maui.SafeAreaEdges.All : Microsoft.Maui.SafeAreaEdges.None;
        }
    }

    private void ApplyShellChromeVisibility()
    {
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);
    }

    /// <summary>Imperatively shows the loading overlay with an optional message.</summary>
    public void ShowLoading(string message = "Loading...")
    {
        var overlay = GetTemplateChild("LoadingOverlay") as LoadingOverlay;
        overlay?.Show(message);
    }

    /// <summary>Imperatively hides the loading overlay.</summary>
    public void HideLoading()
    {
        var overlay = GetTemplateChild("LoadingOverlay") as LoadingOverlay;
        overlay?.Hide();
    }
}
