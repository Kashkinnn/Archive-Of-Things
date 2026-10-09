using ArchiveOfThings.Data;
using ArchiveOfThings.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArchiveOfThings.Model.Features.Items;

public record ArchiveItemDto(
    Guid Id,
    string Title,
    string? ArchivistNotes,
    DateTime DateAdded,
    Dictionary<string, string> TagValues
);

public record AddItemCommand(
    Guid CollectionId,
    Guid UserId,
    string Title,
    string? ArchivistNotes,
    Dictionary<Guid, string> DynamicTagValues
);

public class ItemsHandler
{
    private readonly ArchiveDbContext _db;

    public ItemsHandler(ArchiveDbContext db) => _db = db;

    public async Task<List<ArchiveItemDto>> GetItemsByCollectionAsync(Guid collectionId)
    {
        var items = await _db.ArchiveItems
            .Include(i => i.TagValues)
            .ThenInclude(tv => tv.TagDefinition)
            .Where(i => i.CollectionId == collectionId)
            .OrderByDescending(i => i.DateAdded)
            .ToListAsync();

        return items.Select(i => new ArchiveItemDto(
            i.Id,
            i.Title,
            i.ArchivistNotes,
            i.DateAdded,
            i.TagValues.ToDictionary(tv => tv.TagDefinition.TagName, tv => tv.Value)
        )).ToList();
    }

    public async Task<Guid> AddItemAsync(AddItemCommand cmd)
    {
        var item = new ArchiveItem
        {
            CollectionId = cmd.CollectionId,
            Title = cmd.Title,
            ArchivistNotes = cmd.ArchivistNotes,
            DateAdded = DateTime.UtcNow,
            TagValues = cmd.DynamicTagValues.Select(kvp => new ItemTagValue
            {
                TagDefinitionId = kvp.Key,
                Value = kvp.Value
            }).ToList()
        };

        _db.ArchiveItems.Add(item);

        var collection = await _db.Collections.FindAsync(cmd.CollectionId);
        if (collection != null)
        {
            collection.UpdatedAt = DateTime.UtcNow;

            _db.ActivityLogs.Add(new ActivityLog
            {
                UserId = cmd.UserId,
                Title = cmd.Title,
                CollectionName = collection.Name,
                Description = cmd.ArchivistNotes ?? "Added new item to collection."
            });
        }

        await _db.SaveChangesAsync();
        return item.Id;
    }
}
