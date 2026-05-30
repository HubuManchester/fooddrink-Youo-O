namespace FoodApp.Services.Interfaces;

/// <summary>
/// Applies in-app font scaling for accessibility (works alongside system font size).
/// </summary>
public interface IFontScaleService
{
    /// <summary>Current multiplier (0.85–1.35).</summary>
    double CurrentScale { get; }

    /// <summary>Updates dynamic font resources and persists preference.</summary>
    Task ApplyScaleAsync(double scale);

    /// <summary>Reads persisted scale or returns 1.0.</summary>
    double ReadSavedScale();
}
