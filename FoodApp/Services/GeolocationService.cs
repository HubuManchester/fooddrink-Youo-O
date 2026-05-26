using FoodApp.Models;
using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodApp.Services;

/// <summary>
/// GPS geolocation with simulated nearby restaurant results from coordinates.
/// </summary>
public class GeolocationService : IGeolocationService
{
    private readonly ILogger<GeolocationService> _logger;

    public GeolocationService(ILogger<GeolocationService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsAvailable =>
        DeviceInfo.Platform == DevicePlatform.Android
        || DeviceInfo.Platform == DevicePlatform.iOS
        || DeviceInfo.Platform == DevicePlatform.WinUI
        || DeviceInfo.Platform == DevicePlatform.MacCatalyst;

    /// <inheritdoc />
    public async Task<bool> RequestPermissionAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        return status == PermissionStatus.Granted;
    }

    /// <inheritdoc />
    public async Task<Location?> GetCurrentLocationAsync(CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            _logger.LogWarning("Geolocation unavailable.");
            return null;
        }

        try
        {
            if (!await RequestPermissionAsync())
            {
                return null;
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            return await Microsoft.Maui.Devices.Sensors.Geolocation.Default.GetLocationAsync(request, cancellationToken);
        }
        catch (FeatureNotSupportedException ex)
        {
            _logger.LogWarning(ex, "Geolocation not supported.");
            return null;
        }
        catch (PermissionException ex)
        {
            _logger.LogWarning(ex, "Location permission denied.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Geolocation error.");
            return null;
        }
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<NearbyPlace>> GetNearbyRestaurantsAsync(Location location, CancellationToken cancellationToken = default)
    {
        // Demo dataset derived from coordinates (production would call Places API).
        var seed = (int)(location.Latitude * 100 + location.Longitude * 100);
        var places = new List<NearbyPlace>
        {
            new() { Name = "Green Bowl Café", Address = "12 Market St", DistanceKm = 0.3 + (seed % 5) * 0.1, Category = "Healthy" },
            new() { Name = "Urban Pizza Kitchen", Address = "48 River Rd", DistanceKm = 0.8 + (seed % 3) * 0.2, Category = "Italian" },
            new() { Name = "Sushi Harbor", Address = "5 Dock Ln", DistanceKm = 1.2 + (seed % 4) * 0.15, Category = "Japanese" },
            new() { Name = "Morning Brew Lab", Address = "90 Central Ave", DistanceKm = 1.5, Category = "Coffee" }
        };
        return Task.FromResult<IReadOnlyList<NearbyPlace>>(places.OrderBy(p => p.DistanceKm).ToList());
    }
}
