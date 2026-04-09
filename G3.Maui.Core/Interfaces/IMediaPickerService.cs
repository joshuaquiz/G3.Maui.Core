using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace G3.Maui.Core.Interfaces;

/// <summary>
/// Abstraction for media picker functionality.
/// </summary>
public interface IMediaPickerService
{
    /// <summary>Gets whether the device supports capturing photos.</summary>
    bool IsCaptureSupported { get; }

    /// <summary>Captures a photo using the device camera.</summary>
    Task<FileResult?> CapturePhotoAsync();

    /// <summary>Picks a single photo from the device gallery.</summary>
    Task<FileResult?> PickPhotoAsync(MediaPickerOptions? options = null);

    /// <summary>Picks multiple photos from the device gallery.</summary>
    Task<IEnumerable<FileResult>> PickPhotosAsync(MediaPickerOptions? options = null);
}
