using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodApp.Services;

/// <summary>
/// Wraps MAUI MediaPicker with permission and availability checks.
/// </summary>
public class CameraService : ICameraService
{
    private readonly ILogger<CameraService> _logger;

    public CameraService(ILogger<CameraService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsCameraAvailable => MediaPicker.Default.IsCaptureSupported;

    /// <inheritdoc />
    public async Task<bool> RequestPermissionAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.Camera>();
        return status == PermissionStatus.Granted;
    }

    /// <inheritdoc />
    public async Task<Stream?> CapturePhotoAsync(CancellationToken cancellationToken = default)
    {
        if (!IsCameraAvailable)
        {
            _logger.LogWarning("Camera capture not supported on this device.");
            return null;
        }

        try
        {
            if (!await RequestPermissionAsync())
            {
                return null;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Capture food"
            });
            if (photo == null)
            {
                return null;
            }

            return await photo.OpenReadAsync();
        }
        catch (FeatureNotSupportedException ex)
        {
            _logger.LogWarning(ex, "Camera feature not supported.");
            return null;
        }
        catch (PermissionException ex)
        {
            _logger.LogWarning(ex, "Camera permission denied.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected camera error.");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<Stream?> PickPhotoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            return photo == null ? null : await photo.OpenReadAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Photo picker failed.");
            return null;
        }
    }
}
