using System.Net;
using PersonalMedia.WebDav.Client.Abstractions;
using PersonalMedia.WebDav.Client.Abstractions.Core;
using PersonalMedia.WebDav.Client.Abstractions.Request;
using PersonalMedia.WebDav.Client.Console.Features.Indexer.Abstractions.Data;
using PersonalMedia.WebDav.Client.Console.Features.Indexer.Abstractions.Services;
using PersonalMedia.WebDav.Client.Console.Features.Indexer.Implementation.Data;

namespace PersonalMedia.WebDav.Client.Console.Features.Indexer.Implementation.Services;

internal sealed class MetaInfoLoader : IMetaInfoLoader
{
    private readonly IWebDavClient _client;

    public MetaInfoLoader(IWebDavClient client)
    {
        _client = client;
    }

    private static FolderItem ToFolderItem(WebDavResource resource, IReadOnlyCollection<IItem> children)
        => new(
            resource.DisplayName,
            resource.CreationDate,
            resource.LastModifiedDate,
            resource.ContentLength,
            GetPathFromUri(resource.Uri),
            resource.ETag,
            children);

    private static FileItem ToFileItem(WebDavResource resource)
        => new(
            resource.DisplayName,
            resource.CreationDate,
            resource.LastModifiedDate,
            resource.ContentLength,
            GetPathFromUri(resource.Uri),
            resource.ETag);

    private static IItem ToItem(WebDavResource r)
        => r.IsCollection ? ToFolderItem(r, Array.Empty<IItem>()) : ToFileItem(r);

    private static string GetPathFromUri(string uri)
    {
        var normalized = WebUtility.UrlDecode(uri.TrimEnd('/'));
        var li = normalized.LastIndexOf("/", StringComparison.Ordinal) + 1;

        return normalized.Remove(li);
    }

    public async Task<Dictionary<string, IItem[]>> LoadFromPath(string path, CancellationToken cancellation)
    {
        var propFindParameters = new PropFindParameters
        {
            Headers = new KeyValuePair<string, string>[]
            {
                new( "Depth", "infinity" ),
            },
            RequestType = PropfindRequestType.NamedProperties
        };

        var response = await _client.PropFind(path, propFindParameters, cancellation);

        return response.Resources
            .Select(ToItem)
            .GroupBy(resource => resource.Path, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Created).ToArray());
    }
}