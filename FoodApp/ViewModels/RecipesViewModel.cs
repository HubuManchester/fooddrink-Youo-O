using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Models;
using FoodApp.Resources.Strings;
using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodApp.ViewModels;

/// <summary>
/// Recipe search and favorites list with swipe-to-delete.
/// </summary>
public partial class RecipesViewModel : BaseViewModel
{
    private readonly IMealApiService _mealApiService;
    private readonly IDatabaseService _databaseService;
    private readonly IImageCacheService _imageCacheService;
    private readonly IConnectivityService _connectivityService;
    private readonly ILogger<RecipesViewModel> _logger;
    private CancellationTokenSource? _searchCts;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    public ObservableCollection<MealSummary> Meals { get; } = new();

    public RecipesViewModel(
        IMealApiService mealApiService,
        IDatabaseService databaseService,
        IImageCacheService imageCacheService,
        IConnectivityService connectivityService,
        ILogger<RecipesViewModel> logger)
    {
        _mealApiService = mealApiService;
        _databaseService = databaseService;
        _imageCacheService = imageCacheService;
        _connectivityService = connectivityService;
        _logger = logger;
        Title = AppResources.TabRecipes;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        await SearchAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (!_connectivityService.IsConnected)
        {
            ErrorMessage = AppResources.ErrorNetwork;
            HasError = true;
            return;
        }

        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        try
        {
            IsBusy = true;
            HasError = false;
            var results = await _mealApiService.SearchMealsAsync(SearchQuery, token);
            var favorites = await _databaseService.GetFavoriteMealsAsync();
            var favIds = favorites.Select(f => f.Id).ToHashSet();

            Meals.Clear();
            foreach (var meal in results.Take(25))
            {
                meal.IsFavorite = favIds.Contains(meal.Id);
                await _imageCacheService.GetCachedPathAsync(meal.ThumbnailUrl, token);
                Meals.Add(meal);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Recipe search failed.");
            ErrorMessage = AppResources.ErrorNetwork;
            HasError = true;
        }
        catch (OperationCanceledException)
        {
            // Ignore cancelled search.
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task OpenDetailAsync(MealSummary meal)
    {
        if (meal == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            nameof(Views.RecipeDetailPage),
            new Dictionary<string, object> { ["MealId"] = meal.Id });
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(MealSummary meal)
    {
        if (meal == null)
        {
            return;
        }

        if (meal.IsFavorite)
        {
            await _databaseService.DeleteFavoriteMealAsync(meal.Id);
            meal.IsFavorite = false;
        }
        else
        {
            await _databaseService.SaveFavoriteMealAsync(meal);
            meal.IsFavorite = true;
        }
    }

    [RelayCommand]
    private async Task DeleteFavoriteAsync(MealSummary meal)
    {
        if (meal == null)
        {
            return;
        }

        await _databaseService.DeleteFavoriteMealAsync(meal.Id);
        Meals.Remove(meal);
    }

    [RelayCommand]
    private async Task RetryAsync() => await SearchAsync();
}
