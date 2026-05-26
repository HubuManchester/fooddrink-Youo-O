using FoodApp.Services.Interfaces;

namespace FoodApp.Services;

/// <summary>
/// Persists and applies application theme via preferences.
/// </summary>
public class ThemeService : IThemeService
{
    private const string DarkModeKey = "nutriscan_dark_mode";

    /// <inheritdoc />
    public AppTheme CurrentTheme => Application.Current?.RequestedTheme ?? AppTheme.Light;

    /// <inheritdoc />
    public Task ApplyThemeAsync(bool isDark)
    {
        Preferences.Set(DarkModeKey, isDark);
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Reads persisted dark mode preference.
    /// </summary>
    public static bool ReadDarkModePreference() => Preferences.Get(DarkModeKey, false);
}
