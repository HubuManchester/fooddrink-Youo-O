using FoodApp.Services.Interfaces;
using FoodApp.Views;

namespace FoodApp;

/// <summary>
/// Shell navigation routes and one-time bootstrap of theme/font from SQLite settings.
/// </summary>
public partial class AppShell : Shell
{
    private readonly IDatabaseService _databaseService;
    private readonly IThemeService _themeService;
    private readonly IFontScaleService _fontScaleService;
    private bool _bootstrapped;

    public AppShell(
        IDatabaseService databaseService,
        IThemeService themeService,
        IFontScaleService fontScaleService)
    {
        _databaseService = databaseService;
        _themeService = themeService;
        _fontScaleService = fontScaleService;

        InitializeComponent();
        Routing.RegisterRoute(nameof(RecipeDetailPage), typeof(RecipeDetailPage));
        Routing.RegisterRoute(nameof(HelpPage), typeof(HelpPage));
        Routing.RegisterRoute(nameof(NearbyPage), typeof(NearbyPage));

        Loaded += OnShellLoaded;
    }

    /// <summary>
    /// Applies saved user settings before the first tab page is shown.
    /// </summary>
    private async void OnShellLoaded(object? sender, EventArgs e)
    {
        if (_bootstrapped)
        {
            return;
        }

        _bootstrapped = true;
        try
        {
            await _databaseService.InitializeAsync();
            var settings = await _databaseService.GetSettingsAsync();
            await _themeService.ApplyThemeAsync(settings.IsDarkMode);
            await _fontScaleService.ApplyScaleAsync(settings.FontScale);
        }
        catch
        {
            // Fallback: preferences-based theme and default font scale already applied in App ctor.
            await _fontScaleService.ApplyScaleAsync(_fontScaleService.ReadSavedScale());
        }
    }
}
