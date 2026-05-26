namespace FoodApp.Models;

/// <summary>
/// Lightweight meal item from TheMealDB API or local favorites.
/// </summary>
public class MealSummary
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
}
