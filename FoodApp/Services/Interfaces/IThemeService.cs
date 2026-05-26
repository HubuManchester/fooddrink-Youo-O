namespace FoodApp.Services.Interfaces;

/// <summary>
/// Applies and persists light/dark theme.
/// </summary>
public interface IThemeService
{
    AppTheme CurrentTheme { get; }
    Task ApplyThemeAsync(bool isDark);
}
