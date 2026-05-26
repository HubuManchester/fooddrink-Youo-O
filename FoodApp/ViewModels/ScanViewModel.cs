using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Models;
using FoodApp.Resources.Strings;
using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodApp.ViewModels;

/// <summary>
/// Food scan page: camera capture + ONNX ML classification.
/// </summary>
public partial class ScanViewModel : BaseViewModel
{
    private readonly ICameraService _cameraService;
    private readonly IFoodMlService _foodMlService;
    private readonly IDatabaseService _databaseService;
    private readonly IFlashlightService _flashlightService;
    private readonly IHapticService _hapticService;
    private readonly ILogger<ScanViewModel> _logger;
    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private bool _isCameraAvailable;

    [ObservableProperty]
    private bool _isFlashlightSupported;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _classificationText = string.Empty;

    [ObservableProperty]
    private ImageSource? _previewImage;

    [ObservableProperty]
    private bool _canCapture = true;

    public ScanViewModel(
        ICameraService cameraService,
        IFoodMlService foodMlService,
        IDatabaseService databaseService,
        IFlashlightService flashlightService,
        IHapticService hapticService,
        ILogger<ScanViewModel> logger)
    {
        _cameraService = cameraService;
        _foodMlService = foodMlService;
        _databaseService = databaseService;
        _flashlightService = flashlightService;
        _hapticService = hapticService;
        _logger = logger;
        Title = AppResources.TabScan;
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        IsCameraAvailable = _cameraService.IsCameraAvailable;
        IsFlashlightSupported = _flashlightService.IsSupported;
        CanCapture = IsCameraAvailable;
        StatusMessage = IsCameraAvailable
            ? "Point camera at food and tap capture."
            : AppResources.CameraUnavailable;
        await _foodMlService.InitializeAsync();
    }

    [RelayCommand]
    private async Task CaptureAndClassifyAsync()
    {
        if (!IsCameraAvailable)
        {
            StatusMessage = AppResources.CameraUnavailable;
            return;
        }

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            IsBusy = true;
            StatusMessage = AppResources.Loading;
            await using var stream = await _cameraService.CapturePhotoAsync(token);
            if (stream == null)
            {
                StatusMessage = AppResources.CameraUnavailable;
                return;
            }

            await ProcessImageAsync(stream, token);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Scan cancelled.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scan failed.");
            StatusMessage = "Something went wrong. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PickFromGalleryAsync()
    {
        try
        {
            IsBusy = true;
            await using var stream = await _cameraService.PickPhotoAsync();
            if (stream == null)
            {
                return;
            }

            await ProcessImageAsync(stream, CancellationToken.None);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ToggleFlashlightAsync()
    {
        if (!IsFlashlightSupported)
        {
            StatusMessage = "Flashlight not supported.";
            return;
        }

        await _flashlightService.ToggleAsync();
        _hapticService.LightImpact();
    }

    private async Task ProcessImageAsync(Stream stream, CancellationToken token)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, token);
        ms.Position = 0;
        PreviewImage = ImageSource.FromStream(() =>
        {
            var copy = new MemoryStream(ms.ToArray());
            return copy;
        });

        var result = await _foodMlService.ClassifyAsync(ms, token);
        ClassificationText = string.Format(
            AppResources.MlResultFormat,
            result.Label,
            result.Confidence);

        var mode = result.UsedOnnxModel ? "ONNX model" : "heuristic fallback";
        StatusMessage = $"Analysis complete ({mode}).";

        var record = new ScanRecord
        {
            FoodLabel = result.Label,
            Confidence = result.Confidence,
            Calories = result.EstimatedCalories,
            ScannedAt = DateTime.UtcNow
        };
        await _databaseService.SaveScanAsync(record);
        _hapticService.Success();
    }
}
