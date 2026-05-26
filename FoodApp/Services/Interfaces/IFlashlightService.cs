namespace FoodApp.Services.Interfaces;

/// <summary>
/// Device flashlight/torch for low-light food scanning.
/// </summary>
public interface IFlashlightService
{
    bool IsSupported { get; }
    Task TurnOnAsync();
    Task TurnOffAsync();
    Task ToggleAsync();
}
