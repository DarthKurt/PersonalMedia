using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace PersonalMedia.WebDav.Client.Console;

internal abstract class BaseToolCommand
{
    protected ILogger Logger { get; }

    protected BaseToolCommand(ILogger logger)
    {
        Logger = logger;
    }

    protected abstract Task<int> OnExecuteInternalAsync(CancellationToken cancellation);

    /// <summary>
    /// Entry point for command.
    /// </summary>
    /// <param name="cancellation">Cancellation from shell.</param>
    /// <returns>Status code.</returns>
    [UsedImplicitly]
    public async Task<int> OnExecuteAsync(CancellationToken cancellation)
    {
        cancellation.Register(OnCancellation);

        return await OnExecuteInternalAsync(cancellation);
    }

    private void OnCancellation()
    {
        // TODO: investigate cancellation stuck
        Logger.LogInformation("Canceling...");
    }
}