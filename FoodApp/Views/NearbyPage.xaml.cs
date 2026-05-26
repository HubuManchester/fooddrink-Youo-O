using FoodApp.ViewModels;

namespace FoodApp.Views;

public partial class NearbyPage : ContentPage
{
    public NearbyPage(NearbyViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is NearbyViewModel vm)
        {
            await vm.RefreshCommand.ExecuteAsync(null);
        }
    }
}
