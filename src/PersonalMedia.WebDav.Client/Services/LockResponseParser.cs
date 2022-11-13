using PersonalMedia.WebDav.Client.Abstractions;
using PersonalMedia.WebDav.Client.Abstractions.Response;
using PersonalMedia.WebDav.Client.Helpers;

namespace PersonalMedia.WebDav.Client.Services;

internal class LockResponseParser : BaseResponseParser, IResponseParser<LockResponse>
{
    public LockResponse Parse(string response, int statusCode, string description)
    {
        if (!XDocumentHelper.TryParse(response, out var document)
            || document.Root == null)
        {
            return new LockResponse(statusCode, description);
        }

        var lockDiscovery = document.Root.LocalNameElement("lockDiscovery", StringComparison.OrdinalIgnoreCase);
        var activeLocks = ParseLockDiscovery(lockDiscovery);
        return new LockResponse(statusCode, description, activeLocks);
    }
}