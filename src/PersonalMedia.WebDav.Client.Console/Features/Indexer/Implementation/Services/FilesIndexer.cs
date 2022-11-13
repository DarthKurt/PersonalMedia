using Microsoft.Extensions.Logging;
using PersonalMedia.WebDav.Client.Console.Features.Indexer.Abstractions.Services;

namespace PersonalMedia.WebDav.Client.Console.Features.Indexer.Implementation.Services;

internal sealed class FilesIndexer : IFilesIndexer
{
    private readonly ILogger<FilesIndexer> _logger;
    private readonly EntityFileSerializer _entityFileSerializer;
    private readonly IMetaInfoLoader _metaInfoLoader;

    public FilesIndexer(
        ILogger<FilesIndexer> logger,
        EntityFileSerializer entityFileSerializer,
        IMetaInfoLoader metaInfoLoader)
    {
        _logger = logger;
        _entityFileSerializer = entityFileSerializer;
        _metaInfoLoader = metaInfoLoader;
    }

    public async Task<bool> IndexServerFilesToJson(string path, string destination, CancellationToken cancellation)
    {
        try
        {
            _logger.LogInformation("Loading files meta information for path '{path}'...", path);
            var items = await _metaInfoLoader.LoadFromPath(path, cancellation);

            _logger.LogInformation("Saving files meta information to '{destination}'...", destination);
            await _entityFileSerializer.SaveFileAsync(items, destination, "index.json");

            _logger.LogInformation("Indexing finished.");

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Operation finished unexpectedly");
            return false;
        }
    }
}