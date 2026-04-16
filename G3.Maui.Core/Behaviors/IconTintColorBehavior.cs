using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace G3.Maui.Core.Behaviors;

/// <summary>
/// Applies a tint color to an <see cref="ImageButton"/> icon on Android and iOS.
/// Replaces <c>CommunityToolkit.Maui.Behaviors.IconTintColorBehavior</c> without
/// requiring the consuming project to enable <c>UseMaui</c>.
/// </summary>
public class IconTintColorBehavior : Behavior<ImageButton>
{
    /// <summary>Bindable property for <see cref="TintColor"/>.</summary>
    public static readonly BindableProperty TintColorProperty =
        BindableProperty.Create(
            nameof(TintColor),
            typeof(Color),
            typeof(IconTintColorBehavior),
            null,
            propertyChanged: OnTintColorChanged);

    /// <summary>The color to tint the image button icon.</summary>
    public Color? TintColor
    {
        get => (Color?)GetValue(TintColorProperty);
        set => SetValue(TintColorProperty, value);
    }

    private ImageButton? _view;

    /// <inheritdoc />
    protected override void OnAttachedTo(ImageButton bindable)
    {
        base.OnAttachedTo(bindable);
        _view = bindable;
        bindable.HandlerChanged += OnHandlerChanged;
    }

    /// <inheritdoc />
    protected override void OnDetachingFrom(ImageButton bindable)
    {
        bindable.HandlerChanged -= OnHandlerChanged;
        _view = null;
        base.OnDetachingFrom(bindable);
    }

    private static void OnTintColorChanged(BindableObject b, object o, object n)
    {
        if (b is IconTintColorBehavior behavior)
        {
            behavior.ApplyTint();
        }
    }

    private void OnHandlerChanged(object? sender, EventArgs e)
        => ApplyTint();

    private void ApplyTint()
    {
        if (_view == null)
        {
            return;
        }

#if ANDROID
        if (_view.Handler?.PlatformView is Android.Widget.ImageView imageView)
        {
            if (TintColor is { } color)
            {
                imageView.SetColorFilter(color.ToPlatform(), Android.Graphics.PorterDuff.Mode.SrcIn!);
            }
            else
            {
                imageView.ClearColorFilter();
            }
        }
#elif IOS || MACCATALYST
        if (_view.Handler?.PlatformView is UIKit.UIButton button)
        {
            button.TintColor = TintColor?.ToPlatform() ?? UIKit.UIColor.White;
        }
#endif
    }
}
