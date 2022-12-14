using PersonalMedia.WebDav.Client.Console.Features.Indexer.Abstractions.Data;

namespace PersonalMedia.WebDav.Client.Console.Features.Indexer.Implementation.Data;

internal sealed record FolderItem(
    string Name,
    DateTime? Created,
    DateTime? Modified,
    long? Size,
    string Path,
    string ETag,
    IReadOnlyCollection<IItem> Children
) : IItem;
