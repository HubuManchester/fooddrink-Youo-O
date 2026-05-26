namespace FoodApp.Services.Interfaces;

/// <summary>
/// Network connectivity status.
/// </summary>
public interface IConnectivityService
{
    bool IsConnected { get; }
}
