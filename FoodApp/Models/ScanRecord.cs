using SQLite;

namespace FoodApp.Models;

/// <summary>
/// SQLite entity for a food scan history entry.
/// </summary>
public class ScanRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string FoodLabel { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public int Calories { get; set; }
    public DateTime ScannedAt { get; set; }
    public string? ImagePath { get; set; }
}
