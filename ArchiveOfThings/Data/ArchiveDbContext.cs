using Microsoft.EntityFrameworkCore;
using ArchiveOfThings.Data.Entities;

namespace ArchiveOfThings.Data;

public class ArchiveDbContext : DbContext
{
    public ArchiveDbContext(DbContextOptions<ArchiveDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<TagDefinition> TagDefinitions => Set<TagDefinition>();
    public DbSet<ArchiveItem> ArchiveItems => Set<ArchiveItem>();
    public DbSet<ItemTagValue> ItemTagValues => Set<ItemTagValue>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Collection>()
            .HasOne(c => c.User)
            .WithMany(u => u.Collections)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TagDefinition>()
            .HasOne(td => td.Collection)
            .WithMany(c => c.TagDefinitions)
            .HasForeignKey(td => td.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ArchiveItem>()
            .HasOne(i => i.Collection)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ItemTagValue>()
            .HasOne(tv => tv.Item)
            .WithMany(i => i.TagValues)
            .HasForeignKey(tv => tv.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ItemTagValue>()
            .HasOne(tv => tv.TagDefinition)
            .WithMany(td => td.TagValues)
            .HasForeignKey(tv => tv.TagDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ActivityLog>()
            .HasOne(a => a.User)
            .WithMany(u => u.ActivityLogs)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}