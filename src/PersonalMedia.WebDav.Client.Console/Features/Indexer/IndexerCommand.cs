using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonalMedia.WebDav.Client.Console.Features.Indexer.Abstractions.Services;

namespace PersonalMedia.WebDav.Client.Console.Features.Indexer;

/// <summary>
/// Declares command for Test Connection feature.
/// </summary>
[Command("index",
    Description = "Download files metadata from WebDav server and save it locally.")]
internal class IndexerCommand: BaseToolCommand
{
    private readonly IServiceScopeFactory _scopeFactory;

    public IndexerCommand(
        [UsedImplicitly] ILogger<IndexerCommand> logger,
        IServiceScopeFactory scopeFactory): base(logger)
    {
        _scopeFactory = scopeFactory;
    }

    [Option(CommandOptionType.SingleValue,
        Description = "Path on the server from where to start indexing.",
        Template = "-p|--path",
        ShowInHelpText = true)]
    [UsedImplicitly]
    public string Path { get; } = string.Empty;

    [Option(CommandOptionType.SingleValue,
        Description = "Path to the local folder where to put the index.json file.",
        Template = "-d|--destination",
        ShowInHelpText = true)]
    [UsedImplicitly]
    public string Destination { get; } = string.Empty;

    protected override async Task<int> OnExecuteInternalAsync(CancellationToken cancellation)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var indexer = scope.ServiceProvider.GetRequiredService<IFilesIndexer>();

        var testResult = await indexer.IndexServerFilesToJson(Path, Destination, cancellation);
        return testResult ? 0 : 1;
    }
}
