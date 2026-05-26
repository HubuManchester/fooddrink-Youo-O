namespace FoodApp.Services.Interfaces;

/// <summary>
/// Microphone speech-to-text for recipe voice notes (advanced).
/// </summary>
public interface ISpeechService
{
    bool IsAvailable { get; }
    Task<bool> RequestPermissionAsync();
    Task<string?> ListenAsync(CancellationToken cancellationToken = default);
}
