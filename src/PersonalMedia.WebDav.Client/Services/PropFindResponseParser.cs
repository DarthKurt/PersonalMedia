using System.Xml.Linq;
using CommunityToolkit.Diagnostics;
using PersonalMedia.WebDav.Client.Abstractions;
using PersonalMedia.WebDav.Client.Abstractions.Core;
using PersonalMedia.WebDav.Client.Abstractions.Response;
using PersonalMedia.WebDav.Client.Helpers;

namespace PersonalMedia.WebDav.Client.Services;

internal class PropFindResponseParser : BaseResponseParser, IResponseParser<PropFindResponse>
{
    public PropFindResponse Parse(string response, int statusCode, string description)
    {
        if (string.IsNullOrEmpty(response))
            return new PropFindResponse(statusCode, description);

        if (!XDocumentHelper.TryParse(response, out var document)
            || document.Root == null)
        {
            return new PropFindResponse(statusCode, description);
        }

        var resources = document.Root.LocalNameElements("response", StringComparison.OrdinalIgnoreCase)
            .Select(ParseResource)
            .ToList();

        return new PropFindResponse(statusCode, description, resources);
    }

    private WebDavResource ParseResource(XElement response)
    {
        var uriValue = response.LocalNameElement("href", StringComparison.OrdinalIgnoreCase)?.Value;
        if (string.IsNullOrWhiteSpace(uriValue))
            return ThrowHelper.ThrowInvalidOperationException<WebDavResource>();

        var propStats = MultiStatusParser.GetPropStats(response);

        return CreateResource(uriValue, propStats);
    }

    private WebDavResource CreateResource(string uri, List<PropStat> propStats)
    {
        var properties = MultiStatusParser.GetProperties(propStats);

        var resourceBuilder = new WebDavResource.Builder()
            .WithActiveLocks(ParseLockDiscovery(FindProp("{DAV:}lockdiscovery", properties)))
            .WithContentLanguage(PropertyValueParser.ParseString(FindProp("{DAV:}getcontentlanguage", properties)))
            .WithContentLength(PropertyValueParser.ParseLong(FindProp("{DAV:}getcontentlength", properties)))
            .WithContentType(PropertyValueParser.ParseString(FindProp("{DAV:}getcontenttype", properties)))
            .WithCreationDate(PropertyValueParser.ParseDateTime(FindProp("{DAV:}creationdate", properties)))
            .WithDisplayName(PropertyValueParser.ParseString(FindProp("{DAV:}displayname", properties)))
            .WithETag(PropertyValueParser.ParseString(FindProp("{DAV:}getetag", properties)))
            .WithLastModifiedDate(PropertyValueParser.ParseDateTime(FindProp("{DAV:}getlastmodified", properties)))
            .WithProperties(properties.Select(x => new WebDavProperty(x.Name, x.GetInnerXml())).ToList())
            .WithPropertyStatuses(MultiStatusParser.GetPropertyStatuses(propStats));

        var isHidden = PropertyValueParser.ParseInteger(FindProp("{DAV:}ishidden", properties)) > 0;

        if (isHidden)
            resourceBuilder.IsHidden();

        var isCollection = PropertyValueParser.ParseInteger(FindProp("{DAV:}iscollection", properties)) > 0
                           || PropertyValueParser.ParseResourceType(FindProp("{DAV:}resourcetype", properties)) == ResourceType.Collection;
        if (isCollection)
        {
            resourceBuilder.IsCollection();
            resourceBuilder.WithUri(uri.TrimEnd('/') + "/");
        }
        else
        {
            resourceBuilder.WithUri(uri);
        }
        return resourceBuilder.Build();
    }

    private static XElement? FindProp(XName name, IEnumerable<XElement> properties)
        => properties.FirstOrDefault(x => x.Name.Equals(name));
}