using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FoodApp.Controls;

/// <summary>
/// Reusable star rating control (minimum 44pt touch targets).
/// </summary>
public partial class RatingBar : ContentView
{
    public static readonly BindableProperty RatingProperty =
        BindableProperty.Create(nameof(Rating), typeof(double), typeof(RatingBar), 0d, propertyChanged: OnRatingChanged);

    public ObservableCollection<int> StarCount { get; } = new() { 1, 2, 3, 4, 5 };

    public double Rating
    {
        get => (double)GetValue(RatingProperty);
        set => SetValue(RatingProperty, value);
    }

    public ICommand SelectRatingCommand { get; }

    public RatingBar()
    {
        InitializeComponent();
        SelectRatingCommand = new Command<int>(r => Rating = r);
        BindingContext = this;
    }

    private static void OnRatingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        // Rating drives parent bindings via Binding Mode=TwoWay on parent page.
    }
}
