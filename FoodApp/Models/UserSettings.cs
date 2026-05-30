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

    /// <summary>In-app font scale multiplier (0.85–1.35).</summary>
    public double FontScale { get; set; } = 1.0;
}
