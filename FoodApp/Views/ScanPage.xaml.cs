using FoodApp.ViewModels;

namespace FoodApp.Views;

public partial class ScanPage : ContentPage
{
    public ScanPage(ScanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ScanViewModel vm)
        {
            await vm.InitializeCommand.ExecuteAsync(null);
        }
    }
}
