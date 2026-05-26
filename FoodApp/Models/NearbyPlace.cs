namespace FoodApp.Models;

/// <summary>
/// Nearby restaurant or café from geolocation search.
/// </summary>
public class NearbyPlace
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public string Category { get; set; } = string.Empty;
}
