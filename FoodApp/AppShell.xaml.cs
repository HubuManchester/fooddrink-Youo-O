using FoodApp.Views;

namespace FoodApp;

/// <summary>
/// Shell navigation routes for modal pages.
/// </summary>
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(RecipeDetailPage), typeof(RecipeDetailPage));
        Routing.RegisterRoute(nameof(HelpPage), typeof(HelpPage));
        Routing.RegisterRoute(nameof(NearbyPage), typeof(NearbyPage));
    }
}
