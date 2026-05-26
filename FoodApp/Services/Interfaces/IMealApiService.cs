using FoodApp.Models;

namespace FoodApp.Services.Interfaces;

/// <summary>
/// Remote meal data from TheMealDB public API.
/// </summary>
public interface IMealApiService
{
    Task<IReadOnlyList<MealSummary>> SearchMealsAsync(string query, CancellationToken cancellationToken = default);
    Task<MealDetail?> GetMealDetailAsync(string mealId, CancellationToken cancellationToken = default);
    Task<MealSummary?> GetRandomMealAsync(CancellationToken cancellationToken = default);
}
