using System.Xml.Linq;
using PersonalMedia.WebDav.Client.Abstractions.Core;
using PersonalMedia.WebDav.Client.Helpers;

namespace PersonalMedia.WebDav.Client.Services;

// TODO: rethink
internal abstract class BaseResponseParser
{
    protected virtual List<ActiveLock> ParseLockDiscovery(XElement? lockDiscovery)
    {
        if (lockDiscovery == null)
            return new List<ActiveLock>();

        return lockDiscovery
            .LocalNameElements("activelock", StringComparison.OrdinalIgnoreCase)
            .Select(x => LockResponseParser.CreateActiveLock(x.Elements().ToList()))
            .ToList();
    }

    private static ActiveLock CreateActiveLock(IReadOnlyCollection<XElement> properties)
    {
        var activeLock =
            new ActiveLock.Builder()
                .WithApplyTo(PropertyValueParser.ParseLockDepth(FindProp("depth", properties)))
                .WithLockScope(PropertyValueParser.ParseLockScope(FindProp("lockscope", properties)))
                .WithLockToken(PropertyValueParser.ParseString(FindProp("locktoken", properties)))
                .WithOwner(PropertyValueParser.ParseOwner(FindProp("owner", properties)))
                .WithLockRoot(PropertyValueParser.ParseString(FindProp("lockroot", properties)))
                .WithTimeout(PropertyValueParser.ParseLockTimeout(FindProp("timeout", properties)))
                .Build();

        return activeLock;
    }

    private static XElement? FindProp(string localName, IEnumerable<XElement> properties)
        => properties.FirstOrDefault(x => x.Name.LocalName.Equals(localName, StringComparison.OrdinalIgnoreCase));
}