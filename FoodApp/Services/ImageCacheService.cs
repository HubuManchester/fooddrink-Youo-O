using System.Security.Cryptography;
using System.Text;
using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodApp.Services;

/// <summary>
/// Disk cache for remote meal thumbnails (similar to FFImageLoading caching).
/// </summary>
public class ImageCacheService : IImageCacheService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ImageCacheService> _logger;
    private readonly string _cacheDir;

    public ImageCacheService(HttpClient httpClient, ILogger<ImageCacheService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cacheDir = Path.Combine(FileSystem.CacheDirectory, "images");
        Directory.CreateDirectory(_cacheDir);
    }

    /// <inheritdoc />
    public async Task<string?> GetCachedPathAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        var fileName = Hash(url) + ".img";
        var path = Path.Combine(_cacheDir, fileName);
        if (File.Exists(path))
        {
            return path;
        }

        try
        {
            var bytes = await _httpClient.GetByteArrayAsync(url, cancellationToken);
            await File.WriteAllBytesAsync(path, bytes, cancellationToken);
            return path;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Image download failed for {Url}", url);
            return null;
        }
    }

    private static string Hash(string input)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash);
    }
}
