using System.Globalization;
using FoodApp.Resources.Strings;
using FoodApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FoodApp;

/// <summary>
/// Application entry: culture, persisted theme, and DI window creation.
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        _services = services;
        ApplyEnglishCulture();
        InitializeComponent();

        // Restore persisted theme (Settings also writes this key on save).
        var isDark = ThemeService.ReadDarkModePreference();
        UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
    }

    /// <summary>
    /// Forces English UI strings for consistent assessment demos.
    /// </summary>
    private static void ApplyEnglishCulture()
    {
        var enUs = CultureInfo.GetCultureInfo("en-US");
        CultureInfo.CurrentCulture = enUs;
        CultureInfo.CurrentUICulture = enUs;
        CultureInfo.DefaultThreadCurrentCulture = enUs;
        CultureInfo.DefaultThreadCurrentUICulture = enUs;
        AppResources.Culture = enUs;
    }

    protected override Window CreateWindow(IActivationState? activationState) =>
        new Window(_services.GetRequiredService<AppShell>());
}
