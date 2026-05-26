namespace FoodApp.Services.Interfaces;

/// <summary>
/// Haptic feedback for confirmations.
/// </summary>
public interface IHapticService
{
    void Success();
    void LightImpact();
}
