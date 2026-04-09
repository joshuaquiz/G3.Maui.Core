using System;
using System.Threading;
using System.Threading.Tasks;
using G3.Maui.Core.Interfaces;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Components.CachedImage;

/// <summary>
/// An image control that downloads and caches images locally via <see cref="IImageCacheService"/>.
/// Cancels in-flight loads when the source changes. Shows a placeholder while loading,
/// then crossfades to the loaded image. If loading fails and <see cref="FallbackSource"/>
/// is set, crossfades to the fallback instead of going blank.
/// Call <see cref="Initialize"/> once at app startup (or use UseG3MauiCoreUI which does it automatically).
/// </summary>
public partial class CachedImage : ContentView
{
    private static IImageCacheService? _cacheService;
    private CancellationTokenSource? _cts;

    /// <summary>Bindable property for <see cref="Source"/>.</summary>
    public static readonly BindableProperty SourceProperty =
        BindableProperty.Create(nameof(Source), typeof(Uri), typeof(CachedImage), null,
            propertyChanged: OnSourceChanged);

    /// <summary>Bindable property for <see cref="Aspect"/>.</summary>
    public static readonly BindableProperty AspectProperty =
        BindableProperty.Create(nameof(Aspect), typeof(Aspect), typeof(CachedImage), Aspect.AspectFill);

    /// <summary>Bindable property for <see cref="CachedSource"/>.</summary>
    public static readonly BindableProperty CachedSourceProperty =
        BindableProperty.Create(nameof(CachedSource), typeof(ImageSource), typeof(CachedImage), null);

    /// <summary>Bindable property for <see cref="IsLoading"/>.</summary>
    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(CachedImage), true);

    /// <summary>Bindable property for <see cref="IsImageLoaded"/>.</summary>
    public static readonly BindableProperty IsImageLoadedProperty =
        BindableProperty.Create(nameof(IsImageLoaded), typeof(bool), typeof(CachedImage), false);

    /// <summary>Bindable property for <see cref="FallbackSource"/>.</summary>
    public static readonly BindableProperty FallbackSourceProperty =
        BindableProperty.Create(nameof(FallbackSource), typeof(ImageSource), typeof(CachedImage), null);

    /// <summary>Bindable property for <see cref="FadeDuration"/>.</summary>
    public static readonly BindableProperty FadeDurationProperty =
        BindableProperty.Create(nameof(FadeDuration), typeof(uint), typeof(CachedImage), 300u);

    /// <summary>The URI of the image to load and cache.</summary>
    public Uri? Source
    {
        get => (Uri?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>How the image is scaled. Defaults to AspectFill.</summary>
    public Aspect Aspect
    {
        get => (Aspect)GetValue(AspectProperty);
        set => SetValue(AspectProperty, value);
    }

    /// <summary>The resolved local image source after caching. Bound to the inner Image control.</summary>
    public ImageSource? CachedSource
    {
        get => (ImageSource?)GetValue(CachedSourceProperty);
        set => SetValue(CachedSourceProperty, value);
    }

    /// <summary>True while the image is being downloaded or read from cache.</summary>
    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>True once the image has been successfully loaded into <see cref="CachedSource"/>.</summary>
    public bool IsImageLoaded
    {
        get => (bool)GetValue(IsImageLoadedProperty);
        set => SetValue(IsImageLoadedProperty, value);
    }

    /// <summary>
    /// Image to display when loading fails. Accepts any MAUI ImageSource (file, resource, URI).
    /// If null (the default), a failed load leaves the control blank.
    /// Example: FallbackSource="broken_image.png"
    /// </summary>
    public ImageSource? FallbackSource
    {
        get => (ImageSource?)GetValue(FallbackSourceProperty);
        set => SetValue(FallbackSourceProperty, value);
    }

    /// <summary>
    /// Duration in milliseconds of the crossfade from placeholder to image. Defaults to 300.
    /// Set to 0 to disable the animation.
    /// </summary>
    public uint FadeDuration
    {
        get => (uint)GetValue(FadeDurationProperty);
        set => SetValue(FadeDurationProperty, value);
    }

    /// <summary>Initializes a new instance of <see cref="CachedImage"/>.</summary>
    public CachedImage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Register the image cache service. Call this once during app startup
    /// (or use <see cref="CoreExtensions.UseG3MauiCoreUI"/> which handles this automatically).
    /// </summary>
    public static void Initialize(IImageCacheService imageCacheService) =>
        _cacheService = imageCacheService;

    private static void OnSourceChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        if (bindable is CachedImage img)
        {
            img.LoadImage();
        }
    }

    private async void LoadImage()
    {
        if (_cts != null)
        {
            await _cts.CancelAsync();
            _cts.Dispose();
        }

        _cts = new CancellationTokenSource();

        // Cancel any in-progress fade and snap back to the loading state.
        Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(CachedImageControl);
        Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(PlaceholderBox);
        PlaceholderBox.Opacity = 1;
        CachedImageControl.Opacity = 0;
        if (Source == null || _cacheService == null)
        {
            IsLoading = false;
            IsImageLoaded = false;
            CachedSource = null;
            return;
        }

        IsLoading = true;
        IsImageLoaded = false;

        try
        {
            await Task.Delay(50, _cts.Token);
        }
        catch (OperationCanceledException)
        {
            IsLoading = false;
            return;
        }

        await LoadImageAsync(_cts.Token);
    }

    private async Task LoadImageAsync(CancellationToken ct)
    {
        try
        {
            var path = await _cacheService!.GetCachedImagePathAsync(Source!, ct);
            if (!string.IsNullOrEmpty(path) && !ct.IsCancellationRequested)
            {
                CachedSource = ImageSource.FromFile(path);
                IsImageLoaded = true;
                await CrossfadeAsync(ct);
            }
        }
        catch (TaskCanceledException) { }
        catch (OperationCanceledException) { }
        catch
        {
            IsImageLoaded = false;
            if (FallbackSource != null && !ct.IsCancellationRequested)
            {
                CachedSource = FallbackSource;
                await CrossfadeAsync(ct);
            }
        }
        finally
        {
            if (!ct.IsCancellationRequested)
            {
                IsLoading = false;
            }
        }
    }

    /// <summary>
    /// Simultaneously fades the image in and the placeholder out.
    /// Aborts silently if the token is cancelled mid-animation.
    /// </summary>
    private async Task CrossfadeAsync(CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
        {
            return;
        }

        var duration = FadeDuration;
        await Task.WhenAll(
            CachedImageControl.FadeToAsync(1, duration),
            PlaceholderBox.FadeToAsync(0, duration));
    }

    /// <inheritdoc />
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler != null && Source != null)
        {
            LoadImage();
        }
    }
}
