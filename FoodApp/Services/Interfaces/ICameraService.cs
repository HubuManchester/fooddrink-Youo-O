namespace FoodApp.Services.Interfaces;

/// <summary>
/// Camera capture with availability checks for emulators.
/// </summary>
public interface ICameraService
{
    bool IsCameraAvailable { get; }
    Task<bool> RequestPermissionAsync();
    Task<Stream?> CapturePhotoAsync(CancellationToken cancellationToken = default);
    Task<Stream?> PickPhotoAsync(CancellationToken cancellationToken = default);
}
