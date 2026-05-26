namespace FoodApp.Helpers;

/// <summary>
/// User input validation helpers with friendly messages from resources.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates calorie goal range 500-10000.
    /// </summary>
    public static bool TryValidateCalorieGoal(string? input, out int value, out string? error)
    {
        error = null;
        value = 0;
        if (string.IsNullOrWhiteSpace(input))
        {
            error = Resources.Strings.AppResources.ErrorRequired;
            return false;
        }

        if (!int.TryParse(input, out value) || value < 500 || value > 10000)
        {
            error = Resources.Strings.AppResources.ErrorCalorieRange;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates portion grams 1-999.
    /// </summary>
    public static bool TryValidatePortion(string? input, out int value, out string? error)
    {
        error = null;
        value = 0;
        if (string.IsNullOrWhiteSpace(input))
        {
            error = Resources.Strings.AppResources.ErrorRequired;
            return false;
        }

        if (!int.TryParse(input, out value) || value < 1 || value > 999)
        {
            error = Resources.Strings.AppResources.ErrorPortionRange;
            return false;
        }

        return true;
    }
}
