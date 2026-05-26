using FoodApp.Services.Interfaces;

namespace FoodApp.Services;

/// <summary>
/// Cross-platform haptic feedback wrapper.
/// </summary>
public class HapticService : IHapticService
{
    /// <inheritdoc />
    public void Success()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch
        {
            // Ignore on platforms without haptics.
        }
    }

    /// <inheritdoc />
    public void LightImpact()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        }
        catch
        {
            // Ignore.
        }
    }
}
