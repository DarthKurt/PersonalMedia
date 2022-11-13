namespace PersonalMedia.WebDav.Client.Console.Features.Indexer.Abstractions.Services;

internal interface IFilesIndexer
{
    Task<bool> IndexServerFilesToJson(string path, string destination, CancellationToken cancellation);
}