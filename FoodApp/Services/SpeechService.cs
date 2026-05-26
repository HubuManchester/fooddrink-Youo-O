using System.Globalization;
using CommunityToolkit.Maui.Media;
using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodApp.Services;

/// <summary>
/// Advanced: microphone + platform speech-to-text for recipe notes.
/// </summary>
public class SpeechService : ISpeechService
{
    private readonly ILogger<SpeechService> _logger;

    public SpeechService(ILogger<SpeechService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsAvailable =>
        DeviceInfo.Platform == DevicePlatform.Android
        || DeviceInfo.Platform == DevicePlatform.iOS
        || DeviceInfo.Platform == DevicePlatform.WinUI;

    /// <inheritdoc />
    public async Task<bool> RequestPermissionAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.Microphone>();
        return status == PermissionStatus.Granted;
    }

    /// <inheritdoc />
    public async Task<string?> ListenAsync(CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            _logger.LogWarning("Speech recognition not available.");
            return null;
        }

        try
        {
            if (!await RequestPermissionAsync())
            {
                return null;
            }

            var speechResult = await SpeechToText.Default.ListenAsync(
                CultureInfo.CurrentCulture,
                new Progress<string>(partial => _logger.LogDebug("Partial: {Text}", partial)),
                cancellationToken);

            var text = speechResult?.Text;
            return string.IsNullOrWhiteSpace(text) ? null : text;
        }
        catch (FeatureNotSupportedException ex)
        {
            _logger.LogWarning(ex, "Speech not supported on platform.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Speech recognition failed.");
            return null;
        }
    }
}
