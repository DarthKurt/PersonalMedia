using Microsoft.EntityFrameworkCore;

namespace PersonalMedia.WebDav.Indexer.Data.SQLite;

/// <summary>
/// Subset of <see cref="EntityState"/>
/// </summary>
public enum EntityModifiedStateKind
{
    Deleted = 2,
    Modified = 3,
    Added = 4
}