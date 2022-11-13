namespace PersonalMedia.WebDav.Client.Console.Features.TestConnection.Abstractions.Services;

internal interface IConnectionTester
{
    Task<bool> RunTests(CancellationToken cancellation);
}