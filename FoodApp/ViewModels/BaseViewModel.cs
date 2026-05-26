using CommunityToolkit.Mvvm.ComponentModel;

namespace FoodApp.ViewModels;

/// <summary>
/// Base ViewModel with busy state and title.
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;
}
