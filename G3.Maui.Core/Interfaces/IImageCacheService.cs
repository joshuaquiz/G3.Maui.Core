using System;
using System.Threading;
using System.Threading.Tasks;

namespace G3.Maui.Core.Interfaces;

/// <summary>
/// Service for caching images locally with size limits and expiration.
/// </summary>
public interface IImageCacheService
{
    /// <summary>Get cached image path, or download and cache if not yet available.</summary>
    Task<string?> GetCachedImagePathAsync(Uri imageUrl, CancellationToken cancellationToken);

    /// <summary>Clear all cached images.</summary>
    Task ClearCacheAsync();

    /// <summary>Clear expired images older than the specified duration.</summary>
    Task ClearExpiredImagesAsync(TimeSpan maxAge);

    /// <summary>Get current cache size in bytes.</summary>
    Task<long> GetCacheSizeAsync();

    /// <summary>Enforce a cache size limit by removing the oldest images.</summary>
    Task EnforceCacheSizeLimitAsync(long maxSizeBytes);
}
