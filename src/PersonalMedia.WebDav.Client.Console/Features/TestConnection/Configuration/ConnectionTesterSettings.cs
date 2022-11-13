using System.Net;

namespace PersonalMedia.WebDav.Client.Console.Features.TestConnection.Configuration;

internal class ConnectionTesterSettings
{
    public int FileMaxSize { get; set; }
    public int ContentStreamChunkSize { get; set; }
    public int ContentStreamMilestoneFactor { get; set; }
}