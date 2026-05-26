using FoodApp.Services.Interfaces;

namespace FoodApp.Services;

/// <summary>
/// Network connectivity wrapper.
/// </summary>
public class ConnectivityService : IConnectivityService
{
    /// <inheritdoc />
    public bool IsConnected =>
        Connectivity.NetworkAccess == NetworkAccess.Internet;
}
