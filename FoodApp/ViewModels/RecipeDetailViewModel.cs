using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Helpers;
using FoodApp.Resources.Strings;
using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodApp.ViewModels;

/// <summary>
/// Meal detail with portion validation and voice notes.
/// </summary>
[QueryProperty(nameof(MealId), "MealId")]
public partial class RecipeDetailViewModel : BaseViewModel
{
    private readonly IMealApiService _mealApiService;
    private readonly ISpeechService _speechService;
    private readonly IConnectivityService _connectivityService;
    private readonly ILogger<RecipeDetailViewModel> _logger;
    private int _baseCalories;

    [ObservableProperty]
    private string _mealId = string.Empty;

    [ObservableProperty]
    private string _mealName = string.Empty;

    [ObservableProperty]
    private string _instructions = string.Empty;

    [ObservableProperty]
    private string _thumbnailUrl = string.Empty;

    [ObservableProperty]
    private string _portionText = "100";

    [ObservableProperty]
    private string _portionError = string.Empty;

    [ObservableProperty]
    private string _voiceNote = string.Empty;

    [ObservableProperty]
    private bool _isSpeechAvailable;

    [ObservableProperty]
    private int _estimatedCalories;

    [ObservableProperty]
    private double _rating;

    public RecipeDetailViewModel(
        IMealApiService mealApiService,
        ISpeechService speechService,
        IConnectivityService connectivityService,
        ILogger<RecipeDetailViewModel> logger)
    {
        _mealApiService = mealApiService;
        _speechService = speechService;
        _connectivityService = connectivityService;
        _logger = logger;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (string.IsNullOrEmpty(MealId) || !_connectivityService.IsConnected)
        {
            ErrorMessage = AppResources.ErrorNetwork;
            HasError = true;
            return;
        }

        try
        {
            IsBusy = true;
            IsSpeechAvailable = _speechService.IsAvailable;
            var detail = await _mealApiService.GetMealDetailAsync(MealId);
            if (detail == null)
            {
                ErrorMessage = "Recipe not found.";
                HasError = true;
                return;
            }

            MealName = detail.Name;
            Instructions = detail.Instructions;
            ThumbnailUrl = detail.ThumbnailUrl;
            _baseCalories = detail.EstimatedCalories;
            EstimatedCalories = _baseCalories;
            VoiceNote = detail.VoiceNote;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Load meal detail failed.");
            ErrorMessage = AppResources.ErrorNetwork;
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ListenVoiceNoteAsync()
    {
        if (!_speechService.IsAvailable)
        {
            PortionError = AppResources.MicrophoneUnavailable;
            return;
        }

        PortionError = string.Empty;
        var text = await _speechService.ListenAsync();
        if (!string.IsNullOrWhiteSpace(text))
        {
            VoiceNote = text;
        }
    }

    [RelayCommand]
    private void ApplyPortion()
    {
        if (!ValidationHelper.TryValidatePortion(PortionText, out var grams, out var error))
        {
            PortionError = error ?? AppResources.ErrorPortionRange;
            return;
        }

        PortionError = string.Empty;
        EstimatedCalories = (int)(_baseCalories * (grams / 100.0));
    }

    [RelayCommand]
    private async Task RetryAsync() => await LoadAsync();
}
