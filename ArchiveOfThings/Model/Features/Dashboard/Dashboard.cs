using ArchiveOfThings.Data;
using Microsoft.EntityFrameworkCore;

namespace ArchiveOfThings.Model.Features.Dashboard;

public record DashboardStatsResponse(
    int TotalItems,
    int TotalCollections,
    int RecentlyAddedCount,
    string StorageUsage,
    List<ActivityDto> Activities,
    List<CollectionTileDto> Collections
);

public record ActivityDto(string Title, string CollectionName, string Description, string DateFormatted);
public record CollectionTileDto(Guid Id, string Name, int ItemCount, string Description, string UpdatedFormatted);

public class DashboardHandler
{
    private readonly ArchiveDbContext _db;

    public DashboardHandler(ArchiveDbContext db) => _db = db;

    public async Task<DashboardStatsResponse> GetDashboardDataAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var tenDaysAgo = now.AddDays(-10);
        var twentyFourHoursAgo = now.AddHours(-24);

        var totalItems = await _db.ArchiveItems
            .CountAsync(i => i.Collection.UserId == userId);

        var collections = await _db.Collections
            .Where(c => c.UserId == userId)
            .Select(c => new CollectionTileDto(
                c.Id,
                c.Name,
                c.Items.Count,
                c.ObjectiveDescription,
                c.UpdatedAt.ToLocalTime().ToString("g")))
            .ToListAsync();

        var recentItemsCount = await _db.ArchiveItems
            .CountAsync(i => i.Collection.UserId == userId && i.DateAdded >= twentyFourHoursAgo);

        var activities = await _db.ActivityLogs
            .Where(a => a.UserId == userId && a.CreatedAt >= tenDaysAgo)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new ActivityDto(
                a.Title,
                a.CollectionName,
                a.Description,
                a.CreatedAt.ToString("yyyy-MM-dd")))
            .ToListAsync();

        return new DashboardStatsResponse(
            totalItems,
            collections.Count,
            recentItemsCount,
            "2.4GB / 16GB",
            activities,
            collections
        );
    }
}
