using FoodApp.Models;

namespace FoodApp.Services.Interfaces;

/// <summary>
/// Local SQLite persistence for scans, favorites, and settings.
/// </summary>
public interface IDatabaseService
{
    Task InitializeAsync();
    Task<List<ScanRecord>> GetScanHistoryAsync();
    Task SaveScanAsync(ScanRecord record);
    Task DeleteScanAsync(int id);
    Task<List<MealSummary>> GetFavoriteMealsAsync();
    Task SaveFavoriteMealAsync(MealSummary meal);
    Task DeleteFavoriteMealAsync(string mealId);
    Task<UserSettings> GetSettingsAsync();
    Task SaveSettingsAsync(UserSettings settings);
    Task<List<NutritionDayStat>> GetWeeklyCaloriesAsync();
}
