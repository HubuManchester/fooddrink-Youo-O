namespace FoodApp.Models;

/// <summary>
/// Full meal detail including instructions and nutrition estimate.
/// </summary>
public class MealDetail
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public int EstimatedCalories { get; set; }
    public string VoiceNote { get; set; } = string.Empty;
    public int PortionGrams { get; set; } = 100;
}
