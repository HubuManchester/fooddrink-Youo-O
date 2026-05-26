namespace FoodApp.Services.Interfaces;

/// <summary>
/// Disk-backed image cache (FFImageLoading-style behavior).
/// </summary>
public interface IImageCacheService
{
    Task<string?> GetCachedPathAsync(string url, CancellationToken cancellationToken = default);
}
