using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Behaviors;

/// <summary>
/// Attached property for setting custom keyboard flags on Entry controls.
/// Use <see cref="GetKeyboardFlags"/> / <see cref="SetKeyboardFlags"/> in code,
/// or set KeyboardBehavior.KeyboardFlags="CapitalizeSentence" in XAML.
/// </summary>
public static class KeyboardBehavior
{
    public static readonly BindableProperty KeyboardFlagsProperty =
        BindableProperty.CreateAttached(
            "KeyboardFlags",
            typeof(KeyboardFlags),
            typeof(KeyboardBehavior),
            KeyboardFlags.None,
            propertyChanged: OnKeyboardFlagsChanged);

    /// <summary>Gets the <see cref="KeyboardFlags"/> attached to the given view.</summary>
    public static KeyboardFlags GetKeyboardFlags(BindableObject view)
        => (KeyboardFlags)view.GetValue(KeyboardFlagsProperty);

    /// <summary>Sets the <see cref="KeyboardFlags"/> on the given view.</summary>
    public static void SetKeyboardFlags(
        BindableObject view,
        KeyboardFlags value)
        => view.SetValue(KeyboardFlagsProperty, value);

    private static void OnKeyboardFlagsChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        if (bindable is InputView inputView && newValue is KeyboardFlags flags)
        {
            inputView.Keyboard = Keyboard.Create(flags);
        }
    }
}
