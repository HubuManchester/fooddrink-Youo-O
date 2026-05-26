using FoodApp.Models;

namespace FoodApp.Services.Interfaces;

/// <summary>
/// Food image classification using ONNX Runtime (advanced hardware + ML).
/// </summary>
public interface IFoodMlService
{
    bool IsModelLoaded { get; }
    Task InitializeAsync();
    Task<FoodClassificationResult> ClassifyAsync(Stream imageStream, CancellationToken cancellationToken = default);
}
