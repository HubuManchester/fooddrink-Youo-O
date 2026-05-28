using System.Globalization;
using FoodApp.Models;
using FoodApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using SQLite;

namespace FoodApp.Services;

/// <summary>
/// SQLite-net implementation for local data storage.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly ILogger<DatabaseService> _logger;
    private SQLiteAsyncConnection? _connection;

    public DatabaseService(ILogger<DatabaseService> logger)
    {
        _logger = logger;
    }

    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection != null)
        {
            return _connection;
        }

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "nutriscan.db3");
        _connection = new SQLiteAsyncConnection(dbPath);
        await _connection.CreateTableAsync<ScanRecord>();
        await _connection.CreateTableAsync<FavoriteMealEntity>();
        await _connection.CreateTableAsync<UserSettings>();
        return _connection;
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        try
        {
            await GetConnectionAsync();
            var settings = await GetSettingsAsync();
            if (settings.Id == 0)
            {
                settings.Id = 1;
                await SaveSettingsAsync(settings);
            }
        }
        catch (SQLiteException ex)
        {
            _logger.LogError(ex, "Database initialization failed.");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<ScanRecord>> GetScanHistoryAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<ScanRecord>().OrderByDescending(r => r.ScannedAt).ToListAsync();
    }

    /// <inheritdoc />
    public async Task SaveScanAsync(ScanRecord record)
    {
        var db = await GetConnectionAsync();
        await db.InsertAsync(record);
    }

    /// <inheritdoc />
    public async Task DeleteScanAsync(int id)
    {
        var db = await GetConnectionAsync();
        await db.DeleteAsync<ScanRecord>(id);
    }

    /// <inheritdoc />
    public async Task<List<MealSummary>> GetFavoriteMealsAsync()
    {
        var db = await GetConnectionAsync();
        var rows = await db.Table<FavoriteMealEntity>().ToListAsync();
        return rows.Select(r => new MealSummary
        {
            Id = r.MealId,
            Name = r.Name,
            ThumbnailUrl = r.ThumbnailUrl,
            Category = r.Category,
            IsFavorite = true
        }).ToList();
    }

    /// <inheritdoc />
    public async Task SaveFavoriteMealAsync(MealSummary meal)
    {
        var db = await GetConnectionAsync();
        await db.InsertOrReplaceAsync(new FavoriteMealEntity
        {
            MealId = meal.Id,
            Name = meal.Name,
            ThumbnailUrl = meal.ThumbnailUrl,
            Category = meal.Category
        });
    }

    /// <inheritdoc />
    public async Task DeleteFavoriteMealAsync(string mealId)
    {
        var db = await GetConnectionAsync();
        await db.ExecuteAsync("DELETE FROM FavoriteMealEntity WHERE MealId = ?", mealId);
    }

    /// <inheritdoc />
    public async Task<UserSettings> GetSettingsAsync()
    {
        var db = await GetConnectionAsync();
        var settings = await db.FindAsync<UserSettings>(1);
        return settings ?? new UserSettings { Id = 1, DailyCalorieGoal = 2000 };
    }

    /// <inheritdoc />
    public async Task SaveSettingsAsync(UserSettings settings)
    {
        settings.Id = 1;
        var db = await GetConnectionAsync();
        await db.InsertOrReplaceAsync(settings);
    }

    /// <inheritdoc />
    public async Task<List<NutritionDayStat>> GetWeeklyCaloriesAsync()
    {
        var db = await GetConnectionAsync();
        var scans = await db.Table<ScanRecord>().ToListAsync();
        var stats = new List<NutritionDayStat>();
        for (var i = 6; i >= 0; i--)
        {
            var day = DateTime.Today.AddDays(-i);
            var total = scans
                .Where(s => s.ScannedAt.Date == day.Date)
                .Sum(s => s.Calories);
            stats.Add(new NutritionDayStat
            {
                DayLabel = day.ToString("ddd", CultureInfo.GetCultureInfo("en-US")),
                Calories = total
            });
        }

        return stats;
    }

    [Table("FavoriteMealEntity")]
    private class FavoriteMealEntity
    {
        [PrimaryKey]
        public string MealId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
