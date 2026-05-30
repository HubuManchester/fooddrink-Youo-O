using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using FoodApp.Services;
using FoodApp.Services.Interfaces;
using FoodApp.ViewModels;
using FoodApp.Views;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;

namespace FoodApp;

/// <summary>
/// Application composition root: DI registration and MAUI startup.
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMicrocharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        RegisterServices(builder.Services);
        RegisterViewModels(builder.Services);
        RegisterViews(builder.Services);
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddSingleton<IFoodMlService, OnnxFoodMlService>();
        services.AddSingleton<ICameraService, CameraService>();
        services.AddSingleton<ISpeechService, SpeechService>();
        services.AddSingleton<IShakeService, ShakeService>();
        services.AddSingleton<IGeolocationService, GeolocationService>();
        services.AddSingleton<IHapticService, HapticService>();
        services.AddSingleton<IFlashlightService, FlashlightService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<IFontScaleService, FontScaleService>();
        services.AddSingleton<ITextToSpeechService, TextToSpeechService>();
        services.AddSingleton<IConnectivityService, ConnectivityService>();
        services.AddSingleton<IImageCacheService, ImageCacheService>();

        services.AddHttpClient<IMealApiService, MealApiService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddHttpClient<IImageCacheService, ImageCacheService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<HomeViewModel>();
        services.AddTransient<ScanViewModel>();
        services.AddTransient<RecipesViewModel>();
        services.AddTransient<RecipeDetailViewModel>();
        services.AddTransient<HistoryViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<HelpViewModel>();
        services.AddTransient<NearbyViewModel>();
    }

    private static void RegisterViews(IServiceCollection services)
    {
        services.AddTransient<HomePage>();
        services.AddTransient<ScanPage>();
        services.AddTransient<RecipesPage>();
        services.AddTransient<RecipeDetailPage>();
        services.AddTransient<HistoryPage>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<HelpPage>();
        services.AddTransient<NearbyPage>();
    }
}
