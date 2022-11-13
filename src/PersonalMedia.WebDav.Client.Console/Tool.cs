using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using PersonalMedia.WebDav.Client.Console.Features.Indexer;
using PersonalMedia.WebDav.Client.Console.Features.TestConnection;

namespace PersonalMedia.WebDav.Client.Console;

[Subcommand(typeof(IndexerCommand))]
[Subcommand(typeof(TestConnectionCommand))]
internal sealed class Tool: BaseToolCommand
{
    private readonly CommandLineApplication _application;

    public Tool(
        CommandLineApplication application,
        [UsedImplicitly] ILogger<Tool> logger) : base(logger)
    {
        _application = application;
    }

    protected override Task<int> OnExecuteInternalAsync(CancellationToken cancellation)
    {
        Logger.LogInformation("Nothing to do here...Lets take a look at help.");
        _application.ShowHelp();

        return Task.FromResult(0);
    }
}
