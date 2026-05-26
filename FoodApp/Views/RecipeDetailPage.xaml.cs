using FoodApp.ViewModels;

namespace FoodApp.Views;

public partial class RecipeDetailPage : ContentPage
{
    public RecipeDetailPage(RecipeDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is RecipeDetailViewModel vm)
        {
            await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
