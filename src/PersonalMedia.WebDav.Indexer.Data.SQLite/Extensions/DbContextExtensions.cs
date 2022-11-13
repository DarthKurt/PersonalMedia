using System.Collections.Immutable;
using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PersonalMedia.WebDav.Indexer.Data.SQLite.Extensions;

internal static class DbContextExtensions
{
    public static IReadOnlyList<(EntityModifiedStateKind State, TEntity NewEntity, TEntity OldEntity)>
        GetChangedEntities<TEntity>(this DbContext dbContext) where TEntity : class, new()
    {
        if (!dbContext.ChangeTracker.AutoDetectChangesEnabled)
        {
            // ChangeTracker.Entries() only calls `Try`DetectChanges() behind the scene.
            dbContext.ChangeTracker.DetectChanges();
        }

        return dbContext.ChangeTracker.Entries<TEntity>()
            .Where(IsEntityChanged)
            .Select(entry => ((EntityModifiedStateKind)entry.State, entry.Entity, entry.OriginalValues.CreateWithValues<TEntity>()))
            .ToImmutableArray();
    }

    private static bool IsEntityAddedModifiedOrDeleted(this EntityEntry entry)
        => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;

    private static bool IsReferenceChanged(this ReferenceEntry entry)
        => entry.TargetEntry?.Metadata.IsOwned() == true && entry.TargetEntry.IsEntityChanged();

    private static bool IsEntityChanged(this EntityEntry entry) =>
        entry.IsEntityAddedModifiedOrDeleted()
        || entry.References.Any(r => r.IsReferenceChanged());

    private static T CreateWithValues<T>(this PropertyValues values) where T : new()
    {
        var entity = new T();

        foreach (var prop in values.Properties)
        {
            var value = values[prop.Name];
            if (value is PropertyValues)
                return ThrowHelper.ThrowNotSupportedException<T>("Nested complex object");

            prop.PropertyInfo?.SetValue(entity, value);
        }

        return entity;
    }
}