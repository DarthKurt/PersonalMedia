using PersonalMedia.WebDav.Client.Console.Features.Indexer.Implementation.Data;

namespace PersonalMedia.WebDav.Client.Console.Features.Indexer.Abstractions.Data;

internal sealed record FileItem(string Name, DateTime? Created, DateTime? Modified, long? Size, string Path) : IItem;