using Microsoft.Extensions.Options;

namespace PersonalMedia.WebDav.Client.Console.Features.TestConnection.Configuration;

internal class ConnectionTesterSettingsConfigurator :
    IConfigureOptions<ConnectionTesterSettings>,
    IPostConfigureOptions<ConnectionTesterSettings>
{
    public void Configure(ConnectionTesterSettings connectionTesterSettings)
        => ConfigureCore(connectionTesterSettings);

    public void PostConfigure(string? name, ConnectionTesterSettings connectionTesterSettings)
        => ConfigureCore(connectionTesterSettings);

    private static void ConfigureCore(ConnectionTesterSettings connectionTesterSettings)
    {
        const int defaultFileMaxSize = 100 * 1024 * 1024;
        connectionTesterSettings.FileMaxSize = connectionTesterSettings.FileMaxSize > 0
            ? connectionTesterSettings.FileMaxSize
            : defaultFileMaxSize;

        const int defaultChunk = 81920;
        connectionTesterSettings.ContentStreamChunkSize = connectionTesterSettings.ContentStreamChunkSize > 0
            ? connectionTesterSettings.ContentStreamChunkSize
            : defaultChunk;

        const int defaultMilestoneFactor = 100;
        connectionTesterSettings.ContentStreamMilestoneFactor = connectionTesterSettings.ContentStreamMilestoneFactor > 0
            ? connectionTesterSettings.ContentStreamMilestoneFactor
            : defaultMilestoneFactor;
    }
}