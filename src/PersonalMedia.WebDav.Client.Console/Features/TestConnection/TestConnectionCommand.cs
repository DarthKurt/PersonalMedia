using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using PersonalMedia.WebDav.Client.Console.Features.TestConnection.Abstractions.Services;

namespace PersonalMedia.WebDav.Client.Console.Features.TestConnection;

/// <summary>
/// Declares command for Test Connection feature.
/// </summary>
[Command("test",
    Description = "Run set of tests to check interaction with WebDav server.")]
internal class TestConnectionCommand: BaseToolCommand
{
    private readonly IConnectionTester _tester;

    public TestConnectionCommand(
        [UsedImplicitly] ILogger<TestConnectionCommand> logger,
        IConnectionTester tester): base(logger)
    {
        _tester = tester;
    }

    protected override async Task<int> OnExecuteInternalAsync(CancellationToken cancellation)
    {
        var testResult = await _tester.RunTests(cancellation);
        return testResult ? 0 : 1;
    }
}
