using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using G3.Maui.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using IOPath = System.IO.Path;
using IOFileInfo = System.IO.FileInfo;

namespace G3.Maui.Core.Components.EditableImage;

/// <summary>
/// An image component that supports both view and edit modes.
/// In edit mode the user can tap to pick an image from the gallery, optionally capture a photo
/// with the camera (set <see cref="ShowCameraOption"/> to true), or remove the current image.
/// Control the size limit via <see cref="MaxFileSizeMb"/> and accepted extensions via
/// <see cref="AcceptedFileTypes"/>. After picking, check <see cref="HasImageChanged"/> and
/// read <see cref="SelectedImageFile"/> to upload the file to your storage layer.
/// </summary>
public partial class EditableImageComponent : ContentView
{
    private readonly INotificationService _notificationService;
    private readonly IMediaPickerService _mediaPickerService;
    private readonly ILogger<EditableImageComponent> _logger;
    private Uri? _currentImageUrl;

    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(EditableImageComponent), "Image (Optional)");

    public static readonly BindableProperty ShowLabelProperty =
        BindableProperty.Create(nameof(ShowLabel), typeof(bool), typeof(EditableImageComponent), true);

    public static readonly BindableProperty PlaceholderTextProperty =
        BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(EditableImageComponent), "Tap to Add Image");

    public static readonly BindableProperty HintTextProperty =
        BindableProperty.Create(nameof(HintText), typeof(string), typeof(EditableImageComponent), "JPG or PNG, max 8MB");

    public static readonly BindableProperty ImageHeightProperty =
        BindableProperty.Create(nameof(ImageHeight), typeof(double), typeof(EditableImageComponent), 200.0);

    public static readonly BindableProperty IsEditModeProperty =
        BindableProperty.Create(nameof(IsEditMode), typeof(bool), typeof(EditableImageComponent), false,
            propertyChanged: OnIsEditModeChanged);

    public static readonly BindableProperty ImageUrlProperty =
        BindableProperty.Create(nameof(ImageUrl), typeof(Uri), typeof(EditableImageComponent),
            propertyChanged: OnImageUrlChanged);

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(EditableImageComponent), 0.0,
            propertyChanged: OnCornerRadiusChanged);

    public static readonly BindableProperty ContainerAutomationIdProperty =
        BindableProperty.Create(nameof(ContainerAutomationId), typeof(string), typeof(EditableImageComponent));

    public static readonly BindableProperty SelectImageCommandProperty =
        BindableProperty.Create(nameof(SelectImageCommand), typeof(ICommand), typeof(EditableImageComponent));

    public static readonly BindableProperty RemoveImageCommandProperty =
        BindableProperty.Create(nameof(RemoveImageCommand), typeof(ICommand), typeof(EditableImageComponent));

    public static readonly BindableProperty MaxFileSizeMbProperty =
        BindableProperty.Create(nameof(MaxFileSizeMb), typeof(double), typeof(EditableImageComponent), 8.0);

    public static readonly BindableProperty AcceptedFileTypesProperty =
        BindableProperty.Create(nameof(AcceptedFileTypes), typeof(string[]), typeof(EditableImageComponent),
            new[] { ".jpg", ".jpeg", ".png" });

    public static readonly BindableProperty ShowCameraOptionProperty =
        BindableProperty.Create(nameof(ShowCameraOption), typeof(bool), typeof(EditableImageComponent), false);

    /// <summary>Field label shown above the image area. Defaults to "Image (Optional)".</summary>
    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    /// <summary>When true, shows the field label above the image area. Defaults to true.</summary>
    public bool ShowLabel
    {
        get => (bool)GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    /// <summary>Text shown inside the empty image area in edit mode. Defaults to "Tap to Add Image".</summary>
    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>
    /// Small hint text shown below the image area. Defaults to "JPG or PNG, max 8MB".
    /// Update this when you change <see cref="AcceptedFileTypes"/> or <see cref="MaxFileSizeMb"/>.
    /// </summary>
    public string HintText
    {
        get => (string)GetValue(HintTextProperty);
        set => SetValue(HintTextProperty, value);
    }

    /// <summary>Height of the image area in device-independent units. Defaults to 200.</summary>
    public double ImageHeight
    {
        get => (double)GetValue(ImageHeightProperty);
        set => SetValue(ImageHeightProperty, value);
    }

    /// <summary>When true, the image can be selected or removed. When false, the image is shown read-only.</summary>
    public bool IsEditMode
    {
        get => (bool)GetValue(IsEditModeProperty);
        set => SetValue(IsEditModeProperty, value);
    }

    /// <summary>URI of the currently displayed image. Set this from your view model.</summary>
    public Uri? ImageUrl
    {
        get => (Uri?)GetValue(ImageUrlProperty);
        set => SetValue(ImageUrlProperty, value);
    }

    /// <summary>Corner radius applied to the image container. Defaults to 0.</summary>
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>AutomationId for the image container (for UI testing).</summary>
    public string ContainerAutomationId
    {
        get => (string)GetValue(ContainerAutomationIdProperty);
        set => SetValue(ContainerAutomationIdProperty, value);
    }

    /// <summary>Command that opens the photo picker. Bound internally; do not override.</summary>
    public ICommand SelectImageCommand
    {
        get => (ICommand)GetValue(SelectImageCommandProperty);
        private set => SetValue(SelectImageCommandProperty, value);
    }

    /// <summary>Command that removes the current image. Bound internally; do not override.</summary>
    public ICommand RemoveImageCommand
    {
        get => (ICommand)GetValue(RemoveImageCommandProperty);
        private set => SetValue(RemoveImageCommandProperty, value);
    }

    /// <summary>
    /// Maximum allowed file size in megabytes. Defaults to 8.
    /// Files larger than this are rejected with an error notification.
    /// </summary>
    public double MaxFileSizeMb
    {
        get => (double)GetValue(MaxFileSizeMbProperty);
        set => SetValue(MaxFileSizeMbProperty, value);
    }

    /// <summary>
    /// Lowercase file extensions accepted by the picker. Defaults to { ".jpg", ".jpeg", ".png" }.
    /// Set from code-behind when you need other types, e.g. new[] { ".jpg", ".png", ".webp" }.
    /// Also update <see cref="HintText"/> to match.
    /// </summary>
    public string[] AcceptedFileTypes
    {
        get => (string[])GetValue(AcceptedFileTypesProperty);
        set => SetValue(AcceptedFileTypesProperty, value);
    }

    /// <summary>
    /// When true and the device supports camera capture, tapping the image area shows
    /// an action sheet with "Take Photo" and "Choose from Library" options. Defaults to false.
    /// </summary>
    public bool ShowCameraOption
    {
        get => (bool)GetValue(ShowCameraOptionProperty);
        set => SetValue(ShowCameraOptionProperty, value);
    }

    /// <summary>The file picked by the user, or null if no new image has been selected.</summary>
    public FileResult? SelectedImageFile { get; private set; }

    /// <summary>True if the user has picked a new image since the component was last reset.</summary>
    public bool HasImageChanged => SelectedImageFile != null;

    public EditableImageComponent() : this(Application.Current?.Handler?.MauiContext?.Services) { }

    public EditableImageComponent(IServiceProvider? serviceProvider)
    {
        InitializeComponent();
        _notificationService = serviceProvider?.GetService(typeof(INotificationService)) as INotificationService
            ?? throw new InvalidOperationException("INotificationService not registered");
        _mediaPickerService = serviceProvider?.GetService(typeof(IMediaPickerService)) as IMediaPickerService
            ?? throw new InvalidOperationException("IMediaPickerService not registered");
        _logger = serviceProvider?.GetService(typeof(ILogger<EditableImageComponent>)) as ILogger<EditableImageComponent>
            ?? throw new InvalidOperationException("ILogger<EditableImageComponent> not registered");

        SelectImageCommand = new AsyncRelayCommand(SelectImageAsync);
        RemoveImageCommand = new RelayCommand(RemoveImage);
        UpdateUI();
    }

    private static void OnIsEditModeChanged(
        BindableObject b,
        object o,
        object n)
        => (b as EditableImageComponent)?.UpdateUI();

    private static void OnImageUrlChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is EditableImageComponent c)
        {
            c._currentImageUrl = n as Uri;
            c.PreviewImage.Source = n as Uri;
            c.UpdateUI();
        }
    }

    private static void OnCornerRadiusChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is EditableImageComponent c && n is double r)
        {
            c.ImageContainer.StrokeShape = new RoundRectangle { CornerRadius = new Microsoft.Maui.CornerRadius(r) };
        }
    }

    private void UpdateUI()
    {
        var hasImage = _currentImageUrl != null || SelectedImageFile != null;
        if (IsEditMode)
        {
            ImageContainer.IsEnabled = true;
            ImageContainer.StrokeThickness = 2;
            ImageContainer.BackgroundColor = Application.Current?.RequestedTheme == Microsoft.Maui.ApplicationModel.AppTheme.Dark
                ? Microsoft.Maui.Graphics.Color.FromArgb("#1F1F1F")
                : Microsoft.Maui.Graphics.Color.FromArgb("#F5F5F5");
            ImageClipBorder.IsVisible = hasImage;
            ImagePlaceholder.IsVisible = !hasImage;
            RemoveImageButton.IsVisible = hasImage;
        }
        else
        {
            ImageContainer.IsEnabled = false;
            ImageContainer.StrokeThickness = 0;
            ImageContainer.BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent;
            ImageClipBorder.IsVisible = hasImage;
            ImagePlaceholder.IsVisible = false;
            RemoveImageButton.IsVisible = false;
        }
    }

    private async Task SelectImageAsync()
    {
        if (!IsEditMode)
        {
            return;
        }

        try
        {
            FileResult? result;
            if (ShowCameraOption && _mediaPickerService.IsCaptureSupported)
            {
                var action = await Shell.Current.DisplayActionSheetAsync(null, "Cancel", null, "Take Photo", "Choose from Library");
                if (action == "Take Photo")
                {
                    result = await _mediaPickerService.CapturePhotoAsync();
                }
                else if (action == "Choose from Library")
                {
                    var picked = await _mediaPickerService.PickPhotosAsync(new MediaPickerOptions { Title = "Select Image", SelectionLimit = 1 });
                    result = picked.FirstOrDefault();
                }
                else
                {
                    return;
                }
            }
            else
            {
                var picked = await _mediaPickerService.PickPhotosAsync(new MediaPickerOptions { Title = "Select Image", SelectionLimit = 1 });
                result = picked.FirstOrDefault();
            }
            if (result == null)
            {
                return;
            }

            var maxBytes = (long)(MaxFileSizeMb * 1024 * 1024);
            if (new IOFileInfo(result.FullPath).Length > maxBytes)
            {
                await _notificationService.ShowErrorAsync($"Image must be smaller than {MaxFileSizeMb:0.##}MB", "Image Too Large");
                return;
            }

            var ext = IOPath.GetExtension(result.FileName).ToLowerInvariant();
            if (!AcceptedFileTypes.Contains(ext))
            {
                var allowed = string.Join(", ", AcceptedFileTypes);
                await _notificationService.ShowErrorAsync($"Only {allowed} files are supported", "Invalid File Type");
                return;
            }

            SelectedImageFile = result;
            var stream = await result.OpenReadAsync();
            PreviewImage.Source = ImageSource.FromStream(() => stream);
            UpdateUI();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error picking image");
            await _notificationService.ShowErrorAsync("Failed to select image", "Error");
        }
    }

    private void RemoveImage()
    {
        SelectedImageFile = null;
        _currentImageUrl = null;
        PreviewImage.Source = null;
        UpdateUI();
    }

    /// <summary>Clears the selected image and resets the component to its empty state.</summary>
    public void ClearImage() => RemoveImage();
}
