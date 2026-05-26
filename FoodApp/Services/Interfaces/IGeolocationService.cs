using FoodApp.Models;

namespace FoodApp.Services.Interfaces;

/// <summary>
/// GPS location and nearby place discovery.
/// </summary>
public interface IGeolocationService
{
    bool IsAvailable { get; }
    Task<bool> RequestPermissionAsync();
    Task<Location?> GetCurrentLocationAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NearbyPlace>> GetNearbyRestaurantsAsync(Location location, CancellationToken cancellationToken = default);
}
