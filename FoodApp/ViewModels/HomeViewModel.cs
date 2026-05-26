using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Models;
using FoodApp.Resources.Strings;
using FoodApp.Services.Interfaces;
using Microcharts;
using SkiaSharp;

namespace FoodApp.ViewModels;

/// <summary>
/// Home dashboard with nutrition chart and shake-for-random-meal.
/// </summary>
public partial class HomeViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IMealApiService _mealApiService;
    private readonly IShakeService _shakeService;
    private readonly IHapticService _hapticService;
    private readonly IConnectivityService _connectivityService;

    [ObservableProperty]
    private Chart? _caloriesChart;

    [ObservableProperty]
    private MealSummary? _shakeRecommendation;

    [ObservableProperty]
    private string _shakeStatus = AppResources.ShakeHint;

    [ObservableProperty]
    private int _dailyGoal = 2000;

    [ObservableProperty]
    private int _todayCalories;

    public ObservableCollection<NutritionDayStat> WeekStats { get; } = new();

    public HomeViewModel(
        IDatabaseService databaseService,
        IMealApiService mealApiService,
        IShakeService shakeService,
        IHapticService hapticService,
        IConnectivityService connectivityService)
    {
        _databaseService = databaseService;
        _mealApiService = mealApiService;
        _shakeService = shakeService;
        _hapticService = hapticService;
        _connectivityService = connectivityService;
        Title = AppResources.TabHome;
    }

    /// <summary>
    /// Loads chart data and settings when page appears.
    /// </summary>
    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            HasError = false;
            var settings = await _databaseService.GetSettingsAsync();
            DailyGoal = settings.DailyCalorieGoal;
            var stats = await _databaseService.GetWeeklyCaloriesAsync();
            WeekStats.Clear();
            foreach (var s in stats)
            {
                WeekStats.Add(s);
            }

            var lastDay = stats.LastOrDefault();
            TodayCalories = lastDay != null ? (int)lastDay.Calories : 0;
            BuildChart(stats);
        }
        catch (Exception)
        {
            ErrorMessage = AppResources.ErrorNetwork;
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Starts accelerometer listening for shake gesture.
    /// </summary>
    public void StartShakeDetection()
    {
        if (!_shakeService.IsSupported)
        {
            ShakeStatus = "Shake sensor not available on this device.";
            return;
        }

        _shakeService.Start(async () => await OnShakeDetectedAsync());
    }

    /// <summary>
    /// Stops accelerometer to save battery.
    /// </summary>
    public void StopShakeDetection() => _shakeService.Stop();

    private async Task OnShakeDetectedAsync()
    {
        if (!_connectivityService.IsConnected)
        {
            ErrorMessage = AppResources.ErrorNetwork;
            HasError = true;
            return;
        }

        try
        {
            _hapticService.Success();
            ShakeStatus = AppResources.Loading;
            var meal = await _mealApiService.GetRandomMealAsync();
            ShakeRecommendation = meal;
            ShakeStatus = meal == null ? "No meal found. Try again." : $"Today's pick: {meal.Name}";
        }
        catch (HttpRequestException)
        {
            ErrorMessage = AppResources.ErrorNetwork;
            HasError = true;
        }
    }

    [RelayCommand]
    private async Task OpenNearbyAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.NearbyPage));

    [RelayCommand]
    private async Task OpenHelpAsync() =>
        await Shell.Current.GoToAsync(nameof(Views.HelpPage));

    [RelayCommand]
    private async Task RetryAsync() => await LoadAsync();

    private void BuildChart(List<NutritionDayStat> stats)
    {
        var entries = stats.Select(s => new ChartEntry(s.Calories)
        {
            Label = s.DayLabel,
            ValueLabel = s.Calories.ToString("0"),
            Color = SKColor.Parse("#2E7D32")
        }).ToList();

        CaloriesChart = new BarChart
        {
            Entries = entries,
            LabelTextSize = 14,
            ValueLabelTextSize = 12
        };
    }
}
