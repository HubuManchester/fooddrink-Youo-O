namespace FoodApp.Models;

/// <summary>
/// Daily calorie aggregate for chart display.
/// </summary>
public class NutritionDayStat
{
    public string DayLabel { get; set; } = string.Empty;
    public float Calories { get; set; }
}
