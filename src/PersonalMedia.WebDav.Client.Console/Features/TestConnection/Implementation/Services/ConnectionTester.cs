using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonalMedia.WebDav.Client.Abstractions;
using PersonalMedia.WebDav.Client.Abstractions.Core;
using PersonalMedia.WebDav.Client.Abstractions.Request;
using PersonalMedia.WebDav.Client.Console.Features.TestConnection.Abstractions.Services;
using PersonalMedia.WebDav.Client.Console.Features.TestConnection.Configuration;

namespace PersonalMedia.WebDav.Client.Console.Features.TestConnection.Implementation.Services;

internal sealed class ConnectionTester : IConnectionTester
{
    private readonly ConnectionTesterSettings _settings;
    private readonly ILogger<ConnectionTester> _logger;
    private readonly IWebDavClient _client;

    public ConnectionTester(
        ILogger<ConnectionTester> logger,
        IOptions<ConnectionTesterSettings> settings,
        IWebDavClient client)
    {
        _logger = logger;
        _client = client;
        _settings = settings.Value;
    }

    public async Task<bool> RunTests(CancellationToken cancellation)
    {
        try
        {
            _logger.LogInformation("Starting tests...");

            await Task.WhenAll(TestCapabilities(cancellation), TestFile(cancellation));

            _logger.LogInformation("Tests finished.");

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Operation finished unexpectedly");
            return false;
        }
    }

    private async Task TestCapabilities(CancellationToken cancellation)
    {
        var response = await _client.GetCapabilities(cancellation);
        _logger.LogInformation("Response:\n{response}", response);
        _logger.LogInformation("Capabilities:\n{capabilities}", response.Capabilities);
    }

    private async Task TestFile(CancellationToken cancellation)
    {
        // Get files meta
        var propFindParameters = new PropFindParameters
        {
            Headers = new KeyValuePair<string, string>[]
            {
                new( "Depth", "infinity" ),
            },
            RequestType = PropfindRequestType.NamedProperties
        };

        var propFindResponse = await _client.PropFind("/", propFindParameters, cancellation);
        var rnd = new Random(DateTime.UtcNow.Millisecond);

        // Select random file less than desired number
        WebDavResource resource;
        do
        {
            var itemIndex = rnd.Next(0, propFindResponse.Resources.Count - 1);
            resource = propFindResponse.Resources.ElementAt(itemIndex);
        } while (resource.ContentLength > _settings.FileMaxSize);

        // Request file from server
        var getFileParameters = new GetFileParameters
        {
            CancellationToken = cancellation
        };
        using var response = await _client.GetRawFile(resource.Uri, getFileParameters);

        // Get file content in chunks and report progress
        var size = await ReadChunkAsync(response.Stream, cancellation);

        // Validate result
        _logger.LogInformation("Expected size: {size}, File size: {size}", resource.ContentLength, size);
        if (resource.ContentLength != size)
        {
            _logger.LogWarning("Expected size not equal of the real size");
        }
    }

    private async ValueTask<int> ReadChunkAsync(Stream stream, CancellationToken cancellation)
    {
        var milestoneSize = _settings.ContentStreamChunkSize * _settings.ContentStreamMilestoneFactor;
        var nextMilestone = milestoneSize;
        var buffer = new byte[_settings.ContentStreamChunkSize];
        var readCount = 0;
        int bytesRead;
        do
        {
            if (cancellation.IsCancellationRequested)
                return readCount;

            bytesRead = await stream.ReadAsync(buffer.AsMemory(), cancellation);
            readCount += bytesRead;

            if (readCount <= nextMilestone)
                continue;

            _logger.LogInformation("Read chunk with total of {total} bytes", readCount);
            nextMilestone += milestoneSize;

        } while (bytesRead > 0);


        return readCount;
    }
}