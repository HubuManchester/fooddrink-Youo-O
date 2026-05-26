using FoodApp.ViewModels;

namespace FoodApp.Views;

public partial class HistoryPage : ContentPage
{
    public HistoryPage(HistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is HistoryViewModel vm)
        {
            await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
