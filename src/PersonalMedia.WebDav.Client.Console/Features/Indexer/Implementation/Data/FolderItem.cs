namespace PersonalMedia.WebDav.Client.Console.Features.Indexer.Implementation.Data;

internal sealed record FolderItem(string Name, DateTime? Created, DateTime? Modified, long? Size, string Path, IReadOnlyCollection<IItem> Children) : IItem;