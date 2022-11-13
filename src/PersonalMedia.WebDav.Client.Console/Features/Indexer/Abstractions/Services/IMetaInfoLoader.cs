using PersonalMedia.WebDav.Client.Console.Features.Indexer.Abstractions.Data;

namespace PersonalMedia.WebDav.Client.Console.Features.Indexer.Abstractions.Services;

internal interface IMetaInfoLoader
{
    Task<Dictionary<string, IItem[]>> LoadFromPath(string path, CancellationToken cancellation);
}