namespace FoodApp.Controls;

/// <summary>
/// Reusable nutrition summary card for calories display.
/// </summary>
public partial class NutritionLabel : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(NutritionLabel), string.Empty);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(NutritionLabel), string.Empty);

    public static readonly BindableProperty CaloriesProperty =
        BindableProperty.Create(nameof(Calories), typeof(int), typeof(NutritionLabel), 0);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public int Calories
    {
        get => (int)GetValue(CaloriesProperty);
        set => SetValue(CaloriesProperty, value);
    }

    public NutritionLabel()
    {
        InitializeComponent();
    }
}
