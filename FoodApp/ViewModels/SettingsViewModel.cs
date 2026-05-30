using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Helpers;
using FoodApp.Resources.Strings;
using FoodApp.Services.Interfaces;

namespace FoodApp.ViewModels;

/// <summary>
/// Settings: dark mode, font scale, calorie goal validation, and help navigation.
/// </summary>
public partial class SettingsViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IThemeService _themeService;
    private readonly IFontScaleService _fontScaleService;
    private readonly IHapticService _hapticService;

    [ObservableProperty]
    private bool _isDarkMode;

    [ObservableProperty]
    private string _calorieGoalText = "2000";

    [ObservableProperty]
    private double _fontScale = 1.0;

    [ObservableProperty]
    private string _validationError = string.Empty;

    public SettingsViewModel(
        IDatabaseService databaseService,
        IThemeService themeService,
        IFontScaleService fontScaleService,
        IHapticService hapticService)
    {
        _databaseService = databaseService;
        _themeService = themeService;
        _fontScaleService = fontScaleService;
        _hapticService = hapticService;
        Title = AppResources.TabSettings;
    }

    /// <summary>Loads persisted settings from SQLite.</summary>
    [RelayCommand]
    public async Task LoadAsync()
    {
        var settings = await _databaseService.GetSettingsAsync();
        IsDarkMode = settings.IsDarkMode;
        CalorieGoalText = settings.DailyCalorieGoal.ToString();
        FontScale = settings.FontScale > 0 ? settings.FontScale : 1.0;
    }

    /// <summary>Validates and saves appearance and goal preferences.</summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!ValidationHelper.TryValidateCalorieGoal(CalorieGoalText, out var goal, out var error))
        {
            ValidationError = error ?? AppResources.ErrorCalorieRange;
            return;
        }

        ValidationError = string.Empty;
        var settings = await _databaseService.GetSettingsAsync();
        settings.IsDarkMode = IsDarkMode;
        settings.DailyCalorieGoal = goal;
        settings.FontScale = FontScale;
        await _databaseService.SaveSettingsAsync(settings);
        await _themeService.ApplyThemeAsync(IsDarkMode);
        await _fontScaleService.ApplyScaleAsync(FontScale);
        _hapticService.Success();
        if (Shell.Current?.CurrentPage is Page page)
        {
            await page.DisplayAlert(AppResources.AppName, AppResources.SavedSuccess, AppResources.Ok);
        }
    }
}
