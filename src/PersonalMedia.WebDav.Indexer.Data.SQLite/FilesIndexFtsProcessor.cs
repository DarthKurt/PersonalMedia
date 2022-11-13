using Microsoft.EntityFrameworkCore;
using PersonalMedia.WebDav.Indexer.Data.SQLite.Entity;
using PersonalMedia.WebDav.Indexer.Data.SQLite.Extensions;

namespace PersonalMedia.WebDav.Indexer.Data.SQLite;

internal class FilesIndexFtsProcessor : DatabaseChangeProcessorBase<FilesIndexContext, Item>
{
    public FilesIndexFtsProcessor(FilesIndexContext dbContext)
        : base(dbContext) { }

    protected override void OnAdd(Item newEntity)
    {
        if (ShouldSkipAddedEntity(newEntity))
            return;

        var database = DbContext.Database;
        var normalizedNewName = newEntity.Name.NormalizeText();

        database.ExecuteSql(FtsAddValueQuery(newEntity.Id, normalizedNewName));
    }

    protected override async Task OnAddAsync(Item newEntity, CancellationToken cancellation)
    {
        var database = DbContext.Database;
        var normalizedNewName = newEntity.Name.NormalizeText();

        if (ShouldSkipAddedEntity(newEntity))
            return;

        await database.ExecuteSqlAsync(FtsAddValueQuery(newEntity.Id, normalizedNewName), cancellation);
    }


    protected override void OnUpdate(Item newEntity, Item oldEntity)
    {
        if (ShouldSkipModifiedEntity(newEntity, oldEntity))
            return;

        var database = DbContext.Database;
        var normalizedNewName = newEntity.Name.NormalizeText();
        var normalizedOldName = oldEntity.Name.NormalizeText();

        database.ExecuteSql(FtsAddValueQuery(newEntity.Id, normalizedNewName));
        database.ExecuteSql(FtsRemoveValueQuery(newEntity.Id, normalizedOldName));
    }

    protected override async Task OnUpdateAsync(Item newEntity, Item oldEntity, CancellationToken cancellation)
    {
        if (ShouldSkipModifiedEntity(newEntity, oldEntity))
            return;

        var database = DbContext.Database;
        var normalizedNewName = newEntity.Name.NormalizeText();
        var normalizedOldName = oldEntity.Name.NormalizeText();

        await database.ExecuteSqlAsync(FtsRemoveValueQuery(oldEntity.Id, normalizedOldName), cancellation);
        await database.ExecuteSqlAsync(FtsAddValueQuery(newEntity.Id, normalizedNewName), cancellation);
    }

    protected override void OnDelete(Item oldEntity)
    {
        var database = DbContext.Database;
        var normalizedOldName = oldEntity.Name.NormalizeText();

        database.ExecuteSql(FtsRemoveValueQuery(oldEntity.Id, normalizedOldName));
    }

    protected override async Task OnDeleteAsync(Item oldEntity, CancellationToken cancellation)
    {
        var database = DbContext.Database;
        var normalizedOldName = oldEntity.Name.NormalizeText();

        await database.ExecuteSqlAsync(FtsRemoveValueQuery(oldEntity.Id, normalizedOldName), cancellation);
    }

    private static bool ShouldSkipAddedEntity<TEntity>(TEntity newEntity)
    {
        // TODO: add your logic to avoid indexing this item
        return false;
    }

    private static bool ShouldSkipModifiedEntity<TEntity>(TEntity newEntity, TEntity chapterOld)
    {
        // TODO: add your logic to avoid indexing this item
        return false;
    }

    private static FormattableString FtsAddValueQuery(int id, string nameValue)
        => $"INSERT INTO Items_FTS(RowId, Name) values({id}, {nameValue});";

    private static FormattableString FtsRemoveValueQuery(int id, string nameValue)
        => $"INSERT INTO Items_FTS(Items_FTS, RowId, Name) values('delete', {id}, {nameValue});";
}