using System.Collections.ObjectModel;
using System.Windows.Input;
using G3.Maui.Core.Components.LoadingOverlay;
using G3.Maui.Core.Models;
using G3.Maui.Core.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
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

    public static readonly BindableProperty ShowTopNavBarProperty =
        BindableProperty.Create(nameof(ShowTopNavBar), typeof(bool), typeof(BasePage), true);

    public static readonly BindableProperty ShowBottomNavBarProperty =
        BindableProperty.Create(nameof(ShowBottomNavBar), typeof(bool), typeof(BasePage), false);

    public static readonly BindableProperty NavBarTitleProperty =
        BindableProperty.Create(nameof(NavBarTitle), typeof(string), typeof(BasePage), string.Empty);

    public static readonly BindableProperty ShowNavBarBackButtonProperty =
        BindableProperty.Create(nameof(ShowNavBarBackButton), typeof(bool), typeof(BasePage), true);

    public static readonly BindableProperty NavBarBackCommandProperty =
        BindableProperty.Create(nameof(NavBarBackCommand), typeof(ICommand), typeof(BasePage), null);

    public static readonly BindableProperty NavBarCommandsProperty =
        BindableProperty.Create(nameof(NavBarCommands), typeof(ObservableCollection<NavBarCommand>), typeof(BasePage), null);

    public static readonly BindableProperty NavBarBackgroundColorProperty =
        BindableProperty.Create(nameof(NavBarBackgroundColor), typeof(Color), typeof(BasePage), null);

    public static readonly BindableProperty NavBarIconTintColorProperty =
        BindableProperty.Create(nameof(NavBarIconTintColor), typeof(Color), typeof(BasePage), null);

    /// <summary>
    /// When true, the ad slot row is visible. Set <see cref="AdContent"/> to supply the actual ad view.
    /// </summary>
    public static readonly BindableProperty ShowAdPlaceholderProperty =
        BindableProperty.Create(nameof(ShowAdPlaceholder), typeof(bool), typeof(BasePage), false);

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

    public bool ShowTopNavBar
    {
        get => (bool)GetValue(ShowTopNavBarProperty);
        set => SetValue(ShowTopNavBarProperty, value);
    }

    public bool ShowBottomNavBar
    {
        get => (bool)GetValue(ShowBottomNavBarProperty);
        set => SetValue(ShowBottomNavBarProperty, value);
    }

    public string NavBarTitle
    {
        get => (string)GetValue(NavBarTitleProperty);
        set => SetValue(NavBarTitleProperty, value);
    }

    public bool ShowNavBarBackButton
    {
        get => (bool)GetValue(ShowNavBarBackButtonProperty);
        set => SetValue(ShowNavBarBackButtonProperty, value);
    }

    public ICommand? NavBarBackCommand
    {
        get => (ICommand?)GetValue(NavBarBackCommandProperty);
        set => SetValue(NavBarBackCommandProperty, value);
    }

    public ObservableCollection<NavBarCommand>? NavBarCommands
    {
        get => (ObservableCollection<NavBarCommand>?)GetValue(NavBarCommandsProperty);
        set => SetValue(NavBarCommandsProperty, value);
    }

    public Color? NavBarBackgroundColor
    {
        get => (Color?)GetValue(NavBarBackgroundColorProperty);
        set => SetValue(NavBarBackgroundColorProperty, value);
    }

    public Color? NavBarIconTintColor
    {
        get => (Color?)GetValue(NavBarIconTintColorProperty);
        set => SetValue(NavBarIconTintColorProperty, value);
    }

    public bool ShowAdPlaceholder
    {
        get => (bool)GetValue(ShowAdPlaceholderProperty);
        set => SetValue(ShowAdPlaceholderProperty, value);
    }

    public bool NavBarTitleIsEditable
    {
        get => (bool)GetValue(NavBarTitleIsEditableProperty);
        set => SetValue(NavBarTitleIsEditableProperty, value);
    }

    public View? AdContent
    {
        get => (View?)GetValue(AdContentProperty);
        set => SetValue(AdContentProperty, value);
    }

    public View? BottomBarContent
    {
        get => (View?)GetValue(BottomBarContentProperty);
        set => SetValue(BottomBarContentProperty, value);
    }

    public bool UseSafeArea
    {
        get => (bool)GetValue(UseSafeAreaProperty);
        set => SetValue(UseSafeAreaProperty, value);
    }

    #endregion

    public BasePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyShellChromeVisibility();
        (BindingContext as BaseViewModel)?.ResetPageCancellationToken();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        ApplyShellChromeVisibility();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        (BindingContext as BaseViewModel)?.CancelPageOperations();
    }

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
    /// Prefer this over <see cref="ContentPage.OnAppearing"/> when the logic depends on the
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
            Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(page, useSafeArea);
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
