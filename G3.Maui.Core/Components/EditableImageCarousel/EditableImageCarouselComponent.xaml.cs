using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using G3.Maui.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using IOPath = System.IO.Path;
using IOFileInfo = System.IO.FileInfo;

namespace G3.Maui.Core.Components.EditableImageCarousel;

/// <summary>
/// An item used by <see cref="EditableImageCarouselComponent"/> to represent either
/// an existing image or the "add image" placeholder slot in edit mode.
/// </summary>
public class EditModeImageItem
{
    /// <summary>The URI of the image to display, or null when this is the placeholder slot.</summary>
    public Uri? ImageUrl { get; set; }

    /// <summary>True when this item represents the "add image" placeholder rather than an existing image.</summary>
    public bool IsPlaceholder { get; set; }
}

/// <summary>
/// An image carousel that supports both view and edit modes.
/// In edit mode the user can add images (up to <see cref="MaxImages"/>) and remove existing ones.
/// Optionally enable camera capture with <see cref="ShowCameraOption"/>.
/// Control the size limit via <see cref="MaxFileSizeMb"/> and accepted extensions via
/// <see cref="AcceptedFileTypes"/>. Implement <see cref="OnAddImagesRequested"/> and
/// <see cref="OnRemoveImageRequested"/> to handle storage.
/// </summary>
public partial class EditableImageCarouselComponent : ContentView
{
    private readonly INotificationService _notificationService;
    private readonly IMediaPickerService _mediaPickerService;
    private readonly ILogger<EditableImageCarouselComponent> _logger;

    /// <summary>Bindable property for <see cref="ImageUrls"/>.</summary>
    public static readonly BindableProperty ImageUrlsProperty =
        BindableProperty.Create(nameof(ImageUrls), typeof(ObservableCollection<Uri>), typeof(EditableImageCarouselComponent),
            new ObservableCollection<Uri>(), propertyChanged: OnImageUrlsChanged);

    /// <summary>Bindable property for <see cref="IsEditMode"/>.</summary>
    public static readonly BindableProperty IsEditModeProperty =
        BindableProperty.Create(nameof(IsEditMode), typeof(bool), typeof(EditableImageCarouselComponent), false,
            propertyChanged: OnIsEditModeChanged);

    /// <summary>Bindable property for <see cref="CarouselHeight"/>.</summary>
    public static readonly BindableProperty CarouselHeightProperty =
        BindableProperty.Create(nameof(CarouselHeight), typeof(double), typeof(EditableImageCarouselComponent), 235.0);

    /// <summary>Bindable property for <see cref="MaxImages"/>.</summary>
    public static readonly BindableProperty MaxImagesProperty =
        BindableProperty.Create(nameof(MaxImages), typeof(int), typeof(EditableImageCarouselComponent), 20);

    /// <summary>Bindable property for <see cref="AddImageCommand"/>.</summary>
    public static readonly BindableProperty AddImageCommandProperty =
        BindableProperty.Create(nameof(AddImageCommand), typeof(ICommand), typeof(EditableImageCarouselComponent));

    /// <summary>Bindable property for <see cref="RemoveImageCommand"/>.</summary>
    public static readonly BindableProperty RemoveImageCommandProperty =
        BindableProperty.Create(nameof(RemoveImageCommand), typeof(ICommand), typeof(EditableImageCarouselComponent));

    /// <summary>Bindable property for <see cref="DisplayedImageUrls"/>.</summary>
    public static readonly BindableProperty DisplayedImageUrlsProperty =
        BindableProperty.Create(nameof(DisplayedImageUrls), typeof(List<Uri>), typeof(EditableImageCarouselComponent),
            null, BindingMode.OneWay);

    /// <summary>Bindable property for <see cref="MaxFileSizeMb"/>.</summary>
    public static readonly BindableProperty MaxFileSizeMbProperty =
        BindableProperty.Create(nameof(MaxFileSizeMb), typeof(double), typeof(EditableImageCarouselComponent), 8.0);

    /// <summary>Bindable property for <see cref="AcceptedFileTypes"/>.</summary>
    public static readonly BindableProperty AcceptedFileTypesProperty =
        BindableProperty.Create(nameof(AcceptedFileTypes), typeof(string[]), typeof(EditableImageCarouselComponent),
            new[] { ".jpg", ".jpeg", ".png" });

    /// <summary>Bindable property for <see cref="ShowCameraOption"/>.</summary>
    public static readonly BindableProperty ShowCameraOptionProperty =
        BindableProperty.Create(nameof(ShowCameraOption), typeof(bool), typeof(EditableImageCarouselComponent), false);

    /// <summary>The current collection of image URIs. Bind this two-way from your view model.</summary>
    public ObservableCollection<Uri> ImageUrls
    {
        get => (ObservableCollection<Uri>)GetValue(ImageUrlsProperty);
        set => SetValue(ImageUrlsProperty, value);
    }

    /// <summary>Filtered list of URIs bound to the view-mode carousel. Null when empty.</summary>
    public List<Uri>? DisplayedImageUrls
    {
        get => (List<Uri>?)GetValue(DisplayedImageUrlsProperty);
        set => SetValue(DisplayedImageUrlsProperty, value);
    }

    /// <summary>When true, shows add/remove controls. When false, shows a read-only carousel.</summary>
    public bool IsEditMode
    {
        get => (bool)GetValue(IsEditModeProperty);
        set => SetValue(IsEditModeProperty, value);
    }

    /// <summary>Height of both carousels in device-independent units. Defaults to 235.</summary>
    public double CarouselHeight
    {
        get => (double)GetValue(CarouselHeightProperty);
        set => SetValue(CarouselHeightProperty, value);
    }

    /// <summary>Maximum number of images the user can add. Defaults to 20.</summary>
    public int MaxImages
    {
        get => (int)GetValue(MaxImagesProperty);
        set => SetValue(MaxImagesProperty, value);
    }

    /// <summary>Command that opens the photo picker. Bound internally; do not override.</summary>
    public ICommand AddImageCommand
    {
        get => (ICommand)GetValue(AddImageCommandProperty);
        private set => SetValue(AddImageCommandProperty, value);
    }

    /// <summary>Command that removes an image by URI. Bound internally; do not override.</summary>
    public ICommand RemoveImageCommand
    {
        get => (ICommand)GetValue(RemoveImageCommandProperty);
        private set => SetValue(RemoveImageCommandProperty, value);
    }

    /// <summary>
    /// Maximum allowed file size in megabytes per image. Defaults to 8.
    /// Files larger than this are skipped with an error notification.
    /// </summary>
    public double MaxFileSizeMb
    {
        get => (double)GetValue(MaxFileSizeMbProperty);
        set => SetValue(MaxFileSizeMbProperty, value);
    }

    /// <summary>
    /// Lowercase file extensions accepted by the picker. Defaults to { ".jpg", ".jpeg", ".png" }.
    /// Set from code-behind when you need other types, e.g. new[] { ".jpg", ".png", ".webp" }.
    /// </summary>
    public string[] AcceptedFileTypes
    {
        get => (string[])GetValue(AcceptedFileTypesProperty);
        set => SetValue(AcceptedFileTypesProperty, value);
    }

    /// <summary>
    /// When true and the device supports camera capture, tapping "add" shows an action sheet
    /// with "Take Photo" and "Choose from Library" options. Defaults to false.
    /// </summary>
    public bool ShowCameraOption
    {
        get => (bool)GetValue(ShowCameraOptionProperty);
        set => SetValue(ShowCameraOptionProperty, value);
    }

    /// <summary>Items bound to the edit-mode carousel, including the trailing "add" placeholder slot.</summary>
    public ObservableCollection<EditModeImageItem> EditModeItems { get; } = [];

    /// <summary>
    /// Called when the user picks new images. Receives the validated FileResult list.
    /// Your handler is responsible for uploading and adding URIs to <see cref="ImageUrls"/>.
    /// </summary>
    public Func<IEnumerable<FileResult>, Task>? OnAddImagesRequested { get; set; }

    /// <summary>
    /// Called when the user removes an image. Receives the URI of the image to remove.
    /// Your handler is responsible for deleting the file and removing the URI from <see cref="ImageUrls"/>.
    /// </summary>
    public Func<Uri, Task>? OnRemoveImageRequested { get; set; }

    /// <summary>Initializes a new instance of <see cref="EditableImageCarouselComponent"/>.</summary>
    public EditableImageCarouselComponent() : this(Application.Current?.Handler?.MauiContext?.Services) { }

    /// <summary>Initializes a new instance of <see cref="EditableImageCarouselComponent"/>.</summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
    public EditableImageCarouselComponent(IServiceProvider? serviceProvider)
    {
        InitializeComponent();
        if (serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        _notificationService = serviceProvider.GetRequiredService<INotificationService>();
        _mediaPickerService = serviceProvider.GetRequiredService<IMediaPickerService>();
        _logger = serviceProvider.GetRequiredService<ILogger<EditableImageCarouselComponent>>();

        AddImageCommand = new AsyncRelayCommand(AddImageAsync);
        RemoveImageCommand = new AsyncRelayCommand<Uri>(RemoveImageAsync);

        // Guard against MAUI CarouselView position=-1 bug on Android.
        EditImageCarousel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CarouselView.Position) && EditImageCarousel.Position < 0)
            {
                EditImageCarousel.Position = 0;
            }
        };

        EditImageCarousel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CarouselView.ItemsSource))
            {
                Dispatcher.Dispatch(() =>
                {
                    var src = EditImageCarousel.ItemsSource;
                    if ((src == null || !src.Cast<object>().Any()) && EditImageCarousel.Position != 0)
                    {
                        EditImageCarousel.Position = 0;
                    }
                    else if (src != null && EditImageCarousel.Position < 0)
                    {
                        EditImageCarousel.Position = 0;
                    }
                });
            }
        };

        UpdateUI();
    }

    private static void OnImageUrlsChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is not EditableImageCarouselComponent c)
        {
            return;
        }
        if (o is ObservableCollection<Uri> old)
        {
            old.CollectionChanged -= c.OnCollectionChanged;
        }
        if (n is ObservableCollection<Uri> @new)
        {
            @new.CollectionChanged += c.OnCollectionChanged;
        }

        c.UpdateEditModeItems();
        c.UpdateDisplayedImages();
        c.UpdateUI();
    }

    private void OnCollectionChanged(
        object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        UpdateEditModeItems();
        UpdateDisplayedImages();
        UpdateUI();
    }

    private static void OnIsEditModeChanged(
        BindableObject b,
        object o,
        object n)
        => (b as EditableImageCarouselComponent)?.UpdateUI();

    private void UpdateDisplayedImages()
        => DisplayedImageUrls = ImageUrls?.Count > 0 ? ImageUrls.ToList() : null;

    private void UpdateEditModeItems()
    {
        EditModeItems.Clear();
        foreach (var url in ImageUrls)
        {
            EditModeItems.Add(new EditModeImageItem { ImageUrl = url, IsPlaceholder = false });
        }

        EditModeItems.Add(new EditModeImageItem { ImageUrl = null, IsPlaceholder = true });
    }

    private void UpdateUI()
    {
        ViewModeCarousel.IsVisible = !IsEditMode && DisplayedImageUrls?.Count > 0;
        EditModeCarousel.IsVisible = IsEditMode;
    }

    private async Task AddImageAsync()
    {
        if (!IsEditMode)
        {
            return;
        }

        try
        {
            if (ImageUrls.Count >= MaxImages)
            {
                await _notificationService.ShowErrorAsync($"Maximum of {MaxImages} images allowed", "Image Limit Reached");
                return;
            }

            var remaining = MaxImages - ImageUrls.Count;
            IEnumerable<FileResult> picks;

            if (ShowCameraOption && _mediaPickerService.IsCaptureSupported)
            {
                var action = await Shell.Current.DisplayActionSheetAsync(null, "Cancel", null, "Take Photo", "Choose from Library");
                if (action == "Take Photo")
                {
                    var photo = await _mediaPickerService.CapturePhotoAsync();
                    picks = photo != null ? [photo] : [];
                }
                else if (action == "Choose from Library")
                {
                    picks = await _mediaPickerService.PickPhotosAsync(new MediaPickerOptions { Title = "Select Images", SelectionLimit = remaining });
                }
                else
                {
                    return;
                }
            }
            else
            {
                picks = await _mediaPickerService.PickPhotosAsync(new MediaPickerOptions { Title = "Select Images", SelectionLimit = remaining });
            }

            var selected = picks.ToList();
            if (!selected.Any())
            {
                return;
            }

            var valid = new List<FileResult>();
            var maxBytes = (long)(MaxFileSizeMb * 1024 * 1024);
            foreach (var r in selected.Take(remaining))
            {
                if (new IOFileInfo(r.FullPath).Length > maxBytes)
                {
                    await _notificationService.ShowErrorAsync($"'{r.FileName}' is larger than {MaxFileSizeMb:0.##}MB and will be skipped", "Image Too Large");
                    continue;
                }

                var ext = IOPath.GetExtension(r.FileName).ToLowerInvariant();
                if (!AcceptedFileTypes.Contains(ext))
                {
                    var allowed = string.Join(", ", AcceptedFileTypes);
                    await _notificationService.ShowErrorAsync($"'{r.FileName}' is not a {allowed} file and will be skipped", "Invalid File Type");
                    continue;
                }

                valid.Add(r);
            }
            if (valid.Any() && OnAddImagesRequested != null)
            {
                await OnAddImagesRequested(valid);
            }
            if (selected.Count > remaining)
            {
                await _notificationService.ShowErrorAsync($"Only {remaining} images could be added due to the {MaxImages} image limit", "Image Limit Reached");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error picking images");
            await _notificationService.ShowErrorAsync("Failed to select images", "Error");
        }
    }

    private async Task RemoveImageAsync(Uri? imageUrl)
    {
        if (!IsEditMode || imageUrl == null)
        {
            return;
        }

        try
        {
            if (OnRemoveImageRequested != null)
            {
                await OnRemoveImageRequested(imageUrl);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing image");
            await _notificationService.ShowErrorAsync("Failed to remove image", "Error");
        }
    }
}
