using CommunityToolkit.Mvvm.ComponentModel;
using FoodApp.Resources.Strings;

namespace FoodApp.ViewModels;

/// <summary>
/// Static help content ViewModel.
/// </summary>
public partial class HelpViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _helpText = AppResources.HelpContent;

    public HelpViewModel()
    {
        Title = AppResources.HelpTitle;
    }
}
