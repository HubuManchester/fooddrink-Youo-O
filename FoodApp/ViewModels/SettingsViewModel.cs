using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Helpers;
using FoodApp.Resources.Strings;
using FoodApp.Services.Interfaces;

namespace FoodApp.ViewModels;

/// <summary>
/// Settings: dark mode persistence and calorie goal validation.
/// </summary>
public partial class SettingsViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IThemeService _themeService;
    private readonly IHapticService _hapticService;

    [ObservableProperty]
    private bool _isDarkMode;

    [ObservableProperty]
    private string _calorieGoalText = "2000";

    [ObservableProperty]
    private string _validationError = string.Empty;

    public SettingsViewModel(
        IDatabaseService databaseService,
        IThemeService themeService,
        IHapticService hapticService)
    {
        _databaseService = databaseService;
        _themeService = themeService;
        _hapticService = hapticService;
        Title = AppResources.TabSettings;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        var settings = await _databaseService.GetSettingsAsync();
        IsDarkMode = settings.IsDarkMode;
        CalorieGoalText = settings.DailyCalorieGoal.ToString();
    }

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
        await _databaseService.SaveSettingsAsync(settings);
        await _themeService.ApplyThemeAsync(IsDarkMode);
        _hapticService.Success();
        if (Shell.Current?.CurrentPage is Page page)
        {
            await page.DisplayAlert(AppResources.AppName, AppResources.SavedSuccess, "OK");
        }
    }
}
