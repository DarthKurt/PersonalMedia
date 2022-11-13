using System.Data;
using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace PersonalMedia.WebDav.Indexer.Data.SQLite;

internal abstract class DatabaseChangeProcessorBase<TContext, TEntity>
    where TContext : DbContext
{
    protected TContext DbContext { get; }

    protected DatabaseChangeProcessorBase(TContext dbContext)
    {
        DbContext = dbContext;
    }

    protected abstract void OnAdd(TEntity newEntity);
    protected abstract void OnUpdate(TEntity newEntity, TEntity oldEntity);
    protected abstract void OnDelete(TEntity oldEntity);

    public virtual void ProcessUpdates(IReadOnlyList<(EntityModifiedStateKind State, TEntity NewEntity, TEntity OldEntity)> changedEntities)
    {
        var database = DbContext.Database;

        try
        {
            database.BeginTransaction(IsolationLevel.ReadCommitted);

            foreach (var (state, newEntity, oldEntity) in changedEntities)
            {
                switch (state)
                {
                    case EntityModifiedStateKind.Added:
                        OnAdd(newEntity);
                        break;
                    case EntityModifiedStateKind.Modified:
                        OnUpdate(newEntity, oldEntity);
                        break;
                    case EntityModifiedStateKind.Deleted:
                        OnDelete(oldEntity);
                        break;
                    default:
                        ThrowHelper.ThrowArgumentOutOfRangeException();
                        return;
                }
            }
        }
        finally
        {
            database.CommitTransaction();
        }
    }

    protected abstract Task OnAddAsync(TEntity newEntity, CancellationToken cancellation);
    protected abstract Task OnUpdateAsync(TEntity newEntity, TEntity oldEntity, CancellationToken cancellation);
    protected abstract Task OnDeleteAsync(TEntity oldEntity, CancellationToken cancellation);

    public virtual async Task ProcessUpdatesAsync(
        IReadOnlyList<(EntityModifiedStateKind State, TEntity NewEntity, TEntity OldEntity)> changedEntities,
        CancellationToken cancellation)
    {
        var database = DbContext.Database;

        try
        {
            await database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellation);

            foreach (var (state, newEntity, oldEntity) in changedEntities)
            {
                switch (state)
                {
                    case EntityModifiedStateKind.Added:
                        await OnAddAsync(newEntity, cancellation);
                        break;
                    case EntityModifiedStateKind.Modified:
                        await OnUpdateAsync(newEntity, oldEntity, cancellation);
                        break;
                    case EntityModifiedStateKind.Deleted:
                        await OnDeleteAsync(oldEntity, cancellation);
                        break;
                    default:
                        ThrowHelper.ThrowArgumentOutOfRangeException();
                        return;
                }
            }
        }
        finally
        {
            await database.CommitTransactionAsync(cancellation);
        }
    }
}