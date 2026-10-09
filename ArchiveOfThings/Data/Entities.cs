using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiveOfThings.Data.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Collection> Collections { get; set; } = new List<Collection>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
}

public class Collection
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string ObjectiveDescription { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string IconPath { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public ICollection<TagDefinition> TagDefinitions { get; set; } = new List<TagDefinition>();
    public ICollection<ArchiveItem> Items { get; set; } = new List<ArchiveItem>();
}

public class TagDefinition
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CollectionId { get; set; }

    [Required, MaxLength(50)]
    public string TagName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(CollectionId))]
    public Collection Collection { get; set; } = null!;

    public ICollection<ItemTagValue> TagValues { get; set; } = new List<ItemTagValue>();
}

public class ArchiveItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CollectionId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? ArchivistNotes { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(CollectionId))]
    public Collection Collection { get; set; } = null!;

    public ICollection<ItemTagValue> TagValues { get; set; } = new List<ItemTagValue>();
}

public class ItemTagValue
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ItemId { get; set; }

    [Required]
    public Guid TagDefinitionId { get; set; }

    public string Value { get; set; } = string.Empty;

    [ForeignKey(nameof(ItemId))]
    public ArchiveItem Item { get; set; } = null!;

    [ForeignKey(nameof(TagDefinitionId))]
    public TagDefinition TagDefinition { get; set; } = null!;
}

public class ActivityLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string CollectionName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
