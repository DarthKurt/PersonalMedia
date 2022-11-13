using System.Xml.Linq;
using PersonalMedia.WebDav.Client.Abstractions.Request;

namespace PersonalMedia.WebDav.Client.Helpers;

internal static class SearchRequestBuilder
{
    public static string BuildRequestBody(SearchParameters parameters)
    {
        var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
        var search = new XElement(SearchRequest, new XAttribute(XNamespace.Xmlns + "D", "DAV:"));

        foreach (var ns in parameters.Namespaces)
        {
            var nsAttr = string.IsNullOrEmpty(ns.Prefix)
                ? Xmlns
                : XNamespace.Xmlns + ns.Prefix;
            search.SetAttributeValue(nsAttr, ns.Namespace);
        }

        var basicSearch = new XElement(
            BasicSearch,
            BuildSelect(parameters),
            BuildFrom(parameters),
            BuildWhere(parameters)
        );
        search.Add(basicSearch);

        doc.Add(search);
        return doc.ToStringWithDeclaration();
    }

    private static readonly XName Xmlns = XName.Get("xmlns");
    private static readonly XName BasicSearch = XName.Get("{DAV:}basicsearch");
    private static readonly XName SearchRequest = XName.Get("{DAV:}searchrequest");
    private static readonly XName Select = XName.Get("{DAV:}select");
    private static readonly XName Prop = XName.Get("{DAV:}prop");
    private static readonly XName AllProp = XName.Get("{DAV:}allprop");
    private static readonly XName From = XName.Get("{DAV:}from");
    private static readonly XName Scope = XName.Get("{DAV:}scope");
    private static readonly XName Href = XName.Get("{DAV:}href");
    private static readonly XName Depth = XName.Get("{DAV:}depth");
    private static readonly XName Where = XName.Get("{DAV:}where");
    private static readonly XName Like = XName.Get("{DAV:}like");
    private static readonly XName Literal = XName.Get("{DAV:}literal");

    private static XElement BuildSelect(SearchParameters parameters)
    {
        var selectProperties = parameters.SelectProperties;

        return new XElement(
            Select, selectProperties.Any()
                ? new XElement(Prop, selectProperties.Select(prop => new XElement(prop)).Cast<object>().ToArray())
                : new XElement(AllProp)
        );
    }

    private static XElement BuildFrom(SearchParameters parameters)
        => new(
            From,
            new XElement(
                Scope,
                new XElement(Href, parameters.Scope),
                new XElement(Depth, "infinity")
            )
        );

    private static XElement BuildWhere(SearchParameters parameters)
    {
        return new XElement(
            Where,
            new XElement(
                Like,
                new XElement(Prop, new XElement(parameters.SearchProperty)),
                new XElement(Literal, parameters.SearchKeyword)
            )
        );
    }
}