using FoodApp.ViewModels;

namespace FoodApp.Views;

public partial class RecipesPage : ContentPage
{
    public RecipesPage(RecipesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is RecipesViewModel vm)
        {
            await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
