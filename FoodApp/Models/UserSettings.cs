using SQLite;

namespace FoodApp.Models;

/// <summary>
/// Persisted user preferences (theme, calorie goal).
/// </summary>
public class UserSettings
{
    [PrimaryKey]
    public int Id { get; set; } = 1;

    public bool IsDarkMode { get; set; }
    public int DailyCalorieGoal { get; set; } = 2000;
}
