using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;

namespace G3.Maui.Core.Components.ImageCarousel;

/// <summary>
/// A read-only image carousel for displaying a list of image URLs.
/// Bind <see cref="ImageUrls"/> to a list of URIs.
/// Optionally bind <see cref="TapCommand"/> to handle image taps.
/// Internally guards against the MAUI CarouselView position=-1 crash on Android.
/// </summary>
public partial class ImageCarouselComponent : ContentView
{
    private readonly ILogger<ImageCarouselComponent> _logger;

    /// <summary>Bindable property for <see cref="ImageUrls"/>.</summary>
    public static readonly BindableProperty ImageUrlsProperty =
        BindableProperty.Create(nameof(ImageUrls), typeof(List<Uri>), typeof(ImageCarouselComponent), null,
            propertyChanged: OnImageUrlsChanged);

    /// <summary>Bindable property for <see cref="CarouselHeight"/>.</summary>
    public static readonly BindableProperty CarouselHeightProperty =
        BindableProperty.Create(nameof(CarouselHeight), typeof(double), typeof(ImageCarouselComponent), 235.0);

    /// <summary>Bindable property for <see cref="DisplayedImageUrls"/>.</summary>
    public static readonly BindableProperty DisplayedImageUrlsProperty =
        BindableProperty.Create(nameof(DisplayedImageUrls), typeof(List<Uri>), typeof(ImageCarouselComponent), null,
            defaultBindingMode: BindingMode.OneWay);

    /// <summary>Bindable property for <see cref="TapCommand"/>.</summary>
    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(ImageCarouselComponent), null);

    /// <summary>The list of image URIs to display in the carousel.</summary>
    public List<Uri>? ImageUrls
    {
        get => (List<Uri>?)GetValue(ImageUrlsProperty);
        set => SetValue(ImageUrlsProperty, value);
    }

    /// <summary>Height of the carousel in device-independent units. Defaults to 235.</summary>
    public double CarouselHeight
    {
        get => (double)GetValue(CarouselHeightProperty);
        set => SetValue(CarouselHeightProperty, value);
    }

    /// <summary>Filtered list of URLs actually bound to the carousel (null when the list is empty).</summary>
    public List<Uri>? DisplayedImageUrls
    {
        get => (List<Uri>?)GetValue(DisplayedImageUrlsProperty);
        set => SetValue(DisplayedImageUrlsProperty, value);
    }

    /// <summary>Command executed when the user taps an image. The tapped URI is passed as the parameter.</summary>
    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    /// <summary>Initializes a new instance of <see cref="ImageCarouselComponent"/>.</summary>
    public ImageCarouselComponent() : this(Application.Current?.Handler?.MauiContext?.Services) { }

    /// <summary>Initializes a new instance of <see cref="ImageCarouselComponent"/>.</summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
    public ImageCarouselComponent(IServiceProvider? serviceProvider)
    {
        if (serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        _logger = serviceProvider.GetRequiredService<ILogger<ImageCarouselComponent>>();
        InitializeComponent();

        ImageCarousel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CarouselView.Position) && ImageCarousel.Position < 0)
            {
                _logger.LogWarning("CarouselView Position was {Pos}, resetting to 0", ImageCarousel.Position);
                ImageCarousel.Position = 0;
            }
        };

        ImageCarousel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CarouselView.ItemsSource))
            {
                Dispatcher.Dispatch(() =>
                {
                    var src = ImageCarousel.ItemsSource;
                    if ((src == null || !src.Cast<object>().Any()) && ImageCarousel.Position != 0)
                    {
                        ImageCarousel.Position = 0;
                    }
                    else if (src != null && ImageCarousel.Position < 0)
                    {
                        ImageCarousel.Position = 0;
                    }
                });
            }
        };
    }

    private static void OnImageUrlsChanged(
        BindableObject b,
        object o,
        object n)
        => (b as ImageCarouselComponent)?.UpdateDisplayedImages();

    private void UpdateDisplayedImages()
    {
        DisplayedImageUrls = ImageUrls?.Count > 0 ? ImageUrls.ToList() : null;
    }
}
