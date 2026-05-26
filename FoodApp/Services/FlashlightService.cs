using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodApp.Services;

/// <summary>
/// Flashlight control for low-light food photography.
/// </summary>
public class FlashlightService : IFlashlightService
{
    private readonly ILogger<FlashlightService> _logger;
    private bool _isOn;

    public FlashlightService(ILogger<FlashlightService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsSupported => DeviceInfo.Platform != DevicePlatform.Unknown;

    /// <inheritdoc />
    public async Task TurnOnAsync()
    {
        if (!IsSupported)
        {
            return;
        }

        try
        {
            await Flashlight.Default.TurnOnAsync();
            _isOn = true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Flashlight turn on failed.");
        }
    }

    /// <inheritdoc />
    public async Task TurnOffAsync()
    {
        if (!IsSupported)
        {
            return;
        }

        try
        {
            await Flashlight.Default.TurnOffAsync();
            _isOn = false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Flashlight turn off failed.");
        }
    }

    /// <inheritdoc />
    public async Task ToggleAsync()
    {
        if (_isOn)
        {
            await TurnOffAsync();
        }
        else
        {
            await TurnOnAsync();
        }
    }
}
