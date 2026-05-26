using FoodApp.Services.Interfaces;

namespace FoodApp.Services;

/// <summary>
/// Accelerometer shake detection for random meal recommendation.
/// </summary>
public class ShakeService : IShakeService
{
    private const double ShakeThreshold = 1.78;
    private DateTime _lastShake = DateTime.MinValue;
    private Action? _onShake;

    /// <inheritdoc />
    public bool IsSupported => Accelerometer.Default.IsSupported;

    /// <inheritdoc />
    public void Start(Action onShake)
    {
        if (!IsSupported)
        {
            return;
        }

        _onShake = onShake;
        Accelerometer.Default.ReadingChanged += OnReadingChanged;
        Accelerometer.Default.Start(SensorSpeed.Game);
    }

    /// <inheritdoc />
    public void Stop()
    {
        if (!IsSupported)
        {
            return;
        }

        Accelerometer.Default.ReadingChanged -= OnReadingChanged;
        Accelerometer.Default.Stop();
        _onShake = null;
    }

    private void OnReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        var data = e.Reading;
        var magnitude = Math.Sqrt(
            (data.Acceleration.X * data.Acceleration.X) +
            (data.Acceleration.Y * data.Acceleration.Y) +
            (data.Acceleration.Z * data.Acceleration.Z));

        if (magnitude < ShakeThreshold)
        {
            return;
        }

        var now = DateTime.UtcNow;
        if ((now - _lastShake).TotalMilliseconds < 1200)
        {
            return;
        }

        _lastShake = now;
        _onShake?.Invoke();
    }
}
