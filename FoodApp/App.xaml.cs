using FoodApp.Services;
using FoodApp.Services.Interfaces;

namespace FoodApp;

/// <summary>
/// Application entry: theme bootstrap and database init.
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        var isDark = ThemeService.ReadDarkModePreference();
        UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState) =>
        new(new AppShell());

    protected override async void OnStart()
    {
        base.OnStart();
        try
        {
            var db = Handler?.MauiContext?.Services.GetService<IDatabaseService>();
            if (db != null)
            {
                await db.InitializeAsync();
            }
        }
        catch
        {
            // Logged in service; app must not crash on DB init failure at startup.
        }
    }
}
