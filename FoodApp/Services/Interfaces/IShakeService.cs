namespace FoodApp.Services.Interfaces;

/// <summary>
/// Accelerometer-based shake detection.
/// </summary>
public interface IShakeService
{
    bool IsSupported { get; }
    void Start(Action onShake);
    void Stop();
}
