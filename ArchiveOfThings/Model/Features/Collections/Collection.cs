using ArchiveOfThings.Data;
using ArchiveOfThings.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArchiveOfThings.Model.Features.Collections;

public record CollectionDto(
    Guid Id,
    string Name,
    string Description,
    string IconPath,
    int ItemCount,
    DateTime UpdatedAt
);

public record CreateCollectionCommand(
    Guid UserId,
    string Name,
    string Description,
    string IconPath,
    List<string> CustomTagNames
);

public class CollectionHandler
{
    private readonly ArchiveDbContext _db;

    public CollectionHandler(ArchiveDbContext db) => _db = db;

    public async Task<List<CollectionDto>> GetUserCollectionsAsync(Guid userId)
    {
        return await _db.Collections
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.UpdatedAt)
            .Select(c => new CollectionDto(
                c.Id,
                c.Name,
                c.ObjectiveDescription,
                c.IconPath,
                c.Items.Count,
                c.UpdatedAt))
            .ToListAsync();
    }

    public async Task<Guid> CreateCollectionAsync(CreateCollectionCommand cmd)
    {
        var collection = new Collection
        {
            UserId = cmd.UserId,
            Name = cmd.Name,
            ObjectiveDescription = cmd.Description,
            IconPath = cmd.IconPath,
            TagDefinitions = cmd.CustomTagNames.Select(t => new TagDefinition { TagName = t }).ToList()
        };

        _db.Collections.Add(collection);

        _db.ActivityLogs.Add(new ActivityLog
        {
            UserId = cmd.UserId,
            Title = $"Created Collection: {cmd.Name}",
            CollectionName = cmd.Name,
            Description = $"Initial setup with {cmd.CustomTagNames.Count} custom tags."
        });

        await _db.SaveChangesAsync();
        return collection.Id;
    }
}
