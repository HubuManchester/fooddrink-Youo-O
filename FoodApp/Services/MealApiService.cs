using System.Net.Http.Json;
using System.Text.Json;
using FoodApp.Models;
using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodApp.Services;

/// <summary>
/// TheMealDB public REST API client.
/// </summary>
public class MealApiService : IMealApiService
{
    private const string BaseUrl = "https://www.themealdb.com/api/json/v1/1/";
    private readonly HttpClient _httpClient;
    private readonly ILogger<MealApiService> _logger;

    public MealApiService(HttpClient httpClient, ILogger<MealApiService> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MealSummary>> SearchMealsAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(query)
                ? "search.php?s="
                : $"search.php?s={Uri.EscapeDataString(query)}";
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            return ParseMealList(json);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error searching meals for {Query}", query);
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Meal search timed out.");
            throw new HttpRequestException("Request timed out.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<MealDetail?> GetMealDetailAsync(string mealId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync($"lookup.php?i={mealId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            if (!json.TryGetProperty("meals", out var meals) || meals.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var meal = meals.EnumerateArray().FirstOrDefault();
            if (meal.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            return new MealDetail
            {
                Id = meal.GetProperty("idMeal").GetString() ?? mealId,
                Name = meal.GetProperty("strMeal").GetString() ?? string.Empty,
                Instructions = meal.GetProperty("strInstructions").GetString() ?? string.Empty,
                ThumbnailUrl = meal.GetProperty("strMealThumb").GetString() ?? string.Empty,
                Category = meal.GetProperty("strCategory").GetString() ?? string.Empty,
                Area = meal.GetProperty("strArea").GetString() ?? string.Empty,
                EstimatedCalories = EstimateCalories(meal.GetProperty("strCategory").GetString())
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to load meal {MealId}", mealId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<MealSummary?> GetRandomMealAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync("random.php", cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            var list = ParseMealList(json);
            return list.FirstOrDefault();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch random meal.");
            throw;
        }
    }

    private static List<MealSummary> ParseMealList(JsonElement json)
    {
        var results = new List<MealSummary>();
        if (!json.TryGetProperty("meals", out var meals) || meals.ValueKind != JsonValueKind.Array)
        {
            return results;
        }

        foreach (var meal in meals.EnumerateArray())
        {
            if (meal.ValueKind == JsonValueKind.Null)
            {
                continue;
            }

            results.Add(new MealSummary
            {
                Id = meal.GetProperty("idMeal").GetString() ?? string.Empty,
                Name = meal.GetProperty("strMeal").GetString() ?? string.Empty,
                ThumbnailUrl = meal.GetProperty("strMealThumb").GetString() ?? string.Empty,
                Category = meal.TryGetProperty("strCategory", out var cat)
                    ? cat.GetString() ?? string.Empty
                    : string.Empty
            });
        }

        return results;
    }

    private static int EstimateCalories(string? category) => category?.ToLowerInvariant() switch
    {
        "dessert" => 450,
        "seafood" => 380,
        "vegetarian" => 320,
        "beef" => 650,
        _ => 500
    };
}
