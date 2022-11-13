using System.Net.Http.Headers;
using System.Text;

namespace PersonalMedia.WebDav.Client.Abstractions.Core;

internal static class MediaTypes
{
    internal static readonly MediaTypeHeaderValue XmlMediaType = new MediaTypeHeaderValue("application/xml")
    {
        CharSet = Encoding.UTF8.WebName
    };

    internal static readonly MediaTypeHeaderValue BinaryDataMediaType = new MediaTypeHeaderValue("application/octet-stream");
}