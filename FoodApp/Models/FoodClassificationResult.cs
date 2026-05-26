namespace FoodApp.Models;

/// <summary>
/// Result from ONNX food classifier or heuristic fallback.
/// </summary>
public class FoodClassificationResult
{
    public string Label { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public int EstimatedCalories { get; set; }
    public bool UsedOnnxModel { get; set; }
}
