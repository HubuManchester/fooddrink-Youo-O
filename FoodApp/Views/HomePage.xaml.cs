using FoodApp.ViewModels;

namespace FoodApp.Views;

/// <summary>
/// Home dashboard code-behind (logic in HomeViewModel).
/// </summary>
public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.StartShakeDetection();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopShakeDetection();
    }
}
