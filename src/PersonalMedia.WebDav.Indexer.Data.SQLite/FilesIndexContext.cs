using Microsoft.EntityFrameworkCore;
using PersonalMedia.WebDav.Indexer.Data.SQLite.Entity;
using PersonalMedia.WebDav.Indexer.Data.SQLite.Extensions;

namespace PersonalMedia.WebDav.Indexer.Data.SQLite;

public sealed class FilesIndexContext : DbContext
{
    public FilesIndexContext(DbContextOptions<FilesIndexContext> options)
        : base(options)
    {
    }

    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FilesIndexContext).Assembly);
    }

    public override int SaveChanges()
    {
        var changedEntities = this.GetChangedEntities<Item>();

        // for performance reasons, to avoid calling DetectChanges() again.
        ChangeTracker.AutoDetectChangesEnabled = false;
        var result = base.SaveChanges();
        ChangeTracker.AutoDetectChangesEnabled = true;

        var processor = new FilesIndexFtsProcessor(this);
        processor.ProcessUpdates(changedEntities);

        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var changedEntities = this.GetChangedEntities<Item>();

        // for performance reasons, to avoid calling DetectChanges() again.
        ChangeTracker.AutoDetectChangesEnabled = false;
        var result = await base.SaveChangesAsync(cancellationToken);
        ChangeTracker.AutoDetectChangesEnabled = true;

        var processor = new FilesIndexFtsProcessor(this);
        await processor.ProcessUpdatesAsync(changedEntities, cancellationToken);

        return result;
    }

}
