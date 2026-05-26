using System.Text.Json;
using FoodApp.Models;
using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;

namespace FoodApp.Services;

/// <summary>
/// Advanced: camera image pipeline + ONNX Runtime inference with heuristic fallback.
/// </summary>
public class OnnxFoodMlService : IFoodMlService, IDisposable
{
    private const int InputSize = 224;
    private readonly ILogger<OnnxFoodMlService> _logger;
    private InferenceSession? _session;
    private string[] _labels = Array.Empty<string>();
    private readonly Dictionary<string, int> _calorieMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["apple"] = 95, ["banana"] = 105, ["pizza"] = 285,
        ["salad"] = 150, ["burger"] = 540, ["pasta"] = 400,
        ["sushi"] = 350, ["soup"] = 120, ["sandwich"] = 320, ["steak"] = 680
    };

    public OnnxFoodMlService(ILogger<OnnxFoodMlService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsModelLoaded => _session != null;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        await LoadLabelsAsync();
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("food_classifier.onnx");
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            _session = new InferenceSession(ms.ToArray());
            _logger.LogInformation("ONNX food classifier loaded.");
        }
        catch (FileNotFoundException)
        {
            _logger.LogWarning("food_classifier.onnx not found; using heuristic image analysis fallback.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load ONNX model.");
        }
    }

    /// <inheritdoc />
    public async Task<FoodClassificationResult> ClassifyAsync(Stream imageStream, CancellationToken cancellationToken = default)
    {
        var pixels = await DecodeAndResizeAsync(imageStream, cancellationToken);
        if (_session != null && _labels.Length > 0)
        {
            return RunOnnx(pixels);
        }

        return RunHeuristic(pixels);
    }

    private FoodClassificationResult RunOnnx(float[] pixels)
    {
        var inputName = _session!.InputMetadata.Keys.First();
        var tensor = new DenseTensor<float>(pixels, new[] { 1, 3, InputSize, InputSize });
        using var results = _session.Run(new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor(inputName, tensor)
        });

        var output = results.First().AsEnumerable<float>().ToArray();
        var maxIndex = 0;
        var maxVal = float.MinValue;
        for (var i = 0; i < output.Length; i++)
        {
            if (output[i] > maxVal)
            {
                maxVal = output[i];
                maxIndex = i;
            }
        }

        var label = maxIndex < _labels.Length ? _labels[maxIndex] : "unknown";
        var confidence = Sigmoid(maxVal);
        return BuildResult(label, confidence, usedOnnx: true);
    }

    private FoodClassificationResult RunHeuristic(float[] pixels)
    {
        // Fallback: average RGB channels map to label index (documented for emulator/dev without ONNX file).
        var r = 0f;
        var g = 0f;
        var b = 0f;
        var count = InputSize * InputSize;
        for (var i = 0; i < count; i++)
        {
            r += pixels[i];
            g += pixels[count + i];
            b += pixels[(2 * count) + i];
        }

        r /= count;
        g /= count;
        b /= count;
        var index = (int)((r + g + b) * _labels.Length) % Math.Max(_labels.Length, 1);
        var label = _labels.Length > 0 ? _labels[index] : "salad";
        return BuildResult(label, 0.72f, usedOnnx: false);
    }

    private FoodClassificationResult BuildResult(string label, float confidence, bool usedOnnx)
    {
        var calories = _calorieMap.TryGetValue(label, out var c) ? c : 400;
        return new FoodClassificationResult
        {
            Label = label,
            Confidence = Math.Clamp(confidence, 0f, 1f),
            EstimatedCalories = calories,
            UsedOnnxModel = usedOnnx
        };
    }

    private async Task<float[]> DecodeAndResizeAsync(Stream imageStream, CancellationToken cancellationToken)
    {
        using var skStream = new SKManagedStream(imageStream);
        using var bitmap = SKBitmap.Decode(skStream);
        using var resized = bitmap.Resize(new SKImageInfo(InputSize, InputSize), SKFilterQuality.Medium)
            ?? throw new InvalidOperationException("Could not resize image.");

        var output = new float[3 * InputSize * InputSize];
        for (var y = 0; y < InputSize; y++)
        {
            for (var x = 0; x < InputSize; x++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var color = resized.GetPixel(x, y);
                var offset = (y * InputSize) + x;
                output[offset] = color.Red / 255f;
                output[InputSize * InputSize + offset] = color.Green / 255f;
                output[(2 * InputSize * InputSize) + offset] = color.Blue / 255f;
            }
        }

        return output;
    }

    private async Task LoadLabelsAsync()
    {
        try
        {
            await using var stream = await FileSystem.OpenAppPackageFileAsync("food_labels.json");
            _labels = await JsonSerializer.DeserializeAsync<string[]>(stream) ?? Array.Empty<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not load food labels.");
            _labels = ["salad", "pizza", "burger"];
        }
    }

    private static float Sigmoid(float x) => 1f / (1f + MathF.Exp(-x));

    public void Dispose() => _session?.Dispose();
}
