using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Models;
using FoodApp.Resources.Strings;
using FoodApp.Services.Interfaces;

namespace FoodApp.ViewModels;

/// <summary>
/// Scan history from SQLite with swipe delete.
/// </summary>
public partial class HistoryViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;

    public ObservableCollection<ScanRecord> Records { get; } = new();

    public HistoryViewModel(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
        Title = AppResources.TabHistory;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            var items = await _databaseService.GetScanHistoryAsync();
            Records.Clear();
            foreach (var item in items)
            {
                Records.Add(item);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(ScanRecord record)
    {
        if (record == null)
        {
            return;
        }

        await _databaseService.DeleteScanAsync(record.Id);
        Records.Remove(record);
    }
}
