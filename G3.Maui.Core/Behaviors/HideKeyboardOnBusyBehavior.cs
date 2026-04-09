using System.ComponentModel;
using Microsoft.Maui.Controls;
using G3.Maui.Core.ViewModels;

namespace G3.Maui.Core.Behaviors;

/// <summary>
/// Hides the keyboard when the bound ViewModel's IsBusy property becomes true.
/// Attach to any VisualElement and bind <see cref="ViewModel"/> to a <see cref="BaseViewModel"/>.
/// Useful for preventing the keyboard from obscuring a loading overlay.
/// </summary>
public class HideKeyboardOnBusyBehavior : Behavior<VisualElement>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(
            nameof(ViewModel),
            typeof(INotifyPropertyChanged),
            typeof(HideKeyboardOnBusyBehavior),
            propertyChanged: OnViewModelChanged);

    /// <summary>The view model to monitor. Bind this to the page's BindingContext.</summary>
    public INotifyPropertyChanged? ViewModel
    {
        get => (INotifyPropertyChanged?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private VisualElement? _element;

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);
        _element = bindable;
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        base.OnDetachingFrom(bindable);
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _element = null;
    }

    private static void OnViewModelChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        if (bindable is not HideKeyboardOnBusyBehavior behavior)
        {
            return;
        }
        if (oldValue is INotifyPropertyChanged old)
        {
            old.PropertyChanged -= behavior.OnViewModelPropertyChanged;
        }
        if (newValue is INotifyPropertyChanged @new)
        {
            @new.PropertyChanged += behavior.OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(BaseViewModel.IsBusy) || sender == null)
        {
            return;
        }

        var prop = sender.GetType().GetProperty(nameof(BaseViewModel.IsBusy));
        if (prop?.GetValue(sender) is true)
        {
            HideKeyboard();
        }
    }

    private void HideKeyboard()
    {
        if (_element == null)
        {
            return;
        }
        if (_element is ContentPage { Content: not null } page)
        {
            UnfocusEntries(page.Content);
        }
        else if (_element is View view)
        {
            UnfocusEntries(view);
        }

#if ANDROID
        var ctx = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (ctx != null)
        {
            var imm = (Android.Views.InputMethods.InputMethodManager?)ctx.GetSystemService(Android.Content.Context.InputMethodService);
            if (imm != null && ctx.CurrentFocus != null)
            {
                imm.HideSoftInputFromWindow(ctx.CurrentFocus.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);
            }
        }
#elif IOS || MACCATALYST
        UIKit.UIApplication.SharedApplication.SendAction(
            new ObjCRuntime.Selector("resignFirstResponder"), null, null, null);
#endif
    }

    private static void UnfocusEntries(View view)
    {
        if (view is Entry e)
        {
            e.Unfocus();
            return;
        }
        if (view is Layout layout)
        {
            foreach (var child in layout.Children)
            {
                if (child is View v)
                {
                    UnfocusEntries(v);
                }
            }
        }
        else if (view is ContentView { Content: not null } cv)
        {
            UnfocusEntries(cv.Content);
        }
        else if (view is ScrollView { Content: not null } sv)
        {
            UnfocusEntries(sv.Content);
        }
    }
}
