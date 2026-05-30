using FoodApp.Services.Interfaces;

namespace FoodApp.Services;

/// <summary>
/// Stores font scale in preferences and updates MAUI dynamic resources.
/// </summary>
public class FontScaleService : IFontScaleService
{
    private const string FontScaleKey = "nutriscan_font_scale";
    private const double MinScale = 0.85;
    private const double MaxScale = 1.35;

    /// <inheritdoc />
    public double CurrentScale { get; private set; } = 1.0;

    /// <inheritdoc />
    public double ReadSavedScale()
    {
        var scale = Preferences.Get(FontScaleKey, 1.0);
        return Math.Clamp(scale, MinScale, MaxScale);
    }

    /// <inheritdoc />
    public Task ApplyScaleAsync(double scale)
    {
        scale = Math.Clamp(scale, MinScale, MaxScale);
        CurrentScale = scale;
        Preferences.Set(FontScaleKey, scale);

        if (Application.Current?.Resources != null)
        {
            var resources = Application.Current.Resources;
            resources["ScaledTitleFontSize"] = 24 * scale;
            resources["ScaledSubTitleFontSize"] = 18 * scale;
            resources["ScaledCaptionFontSize"] = 14 * scale;
            resources["ScaledBodyFontSize"] = 16 * scale;
            resources["ScaledHeroFontSize"] = 28 * scale;
            resources["ScaledAppNameFontSize"] = 26 * scale;
        }

        return Task.CompletedTask;
    }
}
