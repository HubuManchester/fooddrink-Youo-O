using FoodApp.Services.Interfaces;

namespace FoodApp.Controls;

/// <summary>
/// Image control that resolves URLs through disk cache service.
/// </summary>
public partial class CachedFoodImage : ContentView
{
    public static readonly BindableProperty UrlProperty =
        BindableProperty.Create(nameof(Url), typeof(string), typeof(CachedFoodImage), string.Empty,
            propertyChanged: OnUrlChanged);

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(CachedFoodImage));

    public string Url
    {
        get => (string)GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }

    public ImageSource? ImageSource
    {
        get => (ImageSource?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public CachedFoodImage()
    {
        InitializeComponent();
    }

    private static async void OnUrlChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CachedFoodImage view || newValue is not string url || string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        try
        {
            var cache = Application.Current?.Handler?.MauiContext?.Services.GetService<IImageCacheService>();
            if (cache != null)
            {
                var path = await cache.GetCachedPathAsync(url);
                if (!string.IsNullOrEmpty(path))
                {
                    view.ImageSource = ImageSource.FromFile(path);
                    return;
                }
            }

            view.ImageSource = ImageSource.FromUri(new Uri(url));
        }
        catch
        {
            view.ImageSource = null;
        }
    }
}
