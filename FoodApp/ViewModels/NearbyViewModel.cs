using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Models;
using FoodApp.Resources.Strings;
using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodApp.ViewModels;

/// <summary>
/// Nearby restaurants using GPS geolocation.
/// </summary>
public partial class NearbyViewModel : BaseViewModel
{
    private readonly IGeolocationService _geolocationService;
    private readonly IHapticService _hapticService;
    private readonly ILogger<NearbyViewModel> _logger;

    [ObservableProperty]
    private bool _isLocationAvailable;

    [ObservableProperty]
    private string _locationStatus = string.Empty;

    public ObservableCollection<NearbyPlace> Places { get; } = new();

    public NearbyViewModel(
        IGeolocationService geolocationService,
        IHapticService hapticService,
        ILogger<NearbyViewModel> logger)
    {
        _geolocationService = geolocationService;
        _hapticService = hapticService;
        _logger = logger;
        Title = AppResources.NearbyRestaurants;
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        IsLocationAvailable = _geolocationService.IsAvailable;
        if (!IsLocationAvailable)
        {
            LocationStatus = AppResources.LocationUnavailable;
            return;
        }

        try
        {
            IsBusy = true;
            LocationStatus = AppResources.Loading;
            var location = await _geolocationService.GetCurrentLocationAsync();
            if (location == null)
            {
                LocationStatus = AppResources.LocationUnavailable;
                return;
            }

            var places = await _geolocationService.GetNearbyRestaurantsAsync(location);
            Places.Clear();
            foreach (var p in places)
            {
                Places.Add(p);
            }

            LocationStatus = $"Lat {location.Latitude:F4}, Lon {location.Longitude:F4}";
            _hapticService.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nearby refresh failed.");
            LocationStatus = AppResources.LocationUnavailable;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
