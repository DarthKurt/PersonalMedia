using System.Xml.Linq;
using PersonalMedia.WebDav.Client.Abstractions.Core;

namespace PersonalMedia.WebDav.Client.Helpers;

internal static class PropfindRequestBuilder
{
    public static string BuildRequest(
        PropfindRequestType requestType,
        IReadOnlyCollection<XName> customProperties,
        IReadOnlyCollection<NamespaceAttr> namespaces)
    {
        return requestType == PropfindRequestType.NamedProperties
            ? BuildNamedPropRequest(customProperties, namespaces)
            : BuildAllPropRequest(customProperties, namespaces);
    }

    private static readonly XName Xmlns = XName.Get("xmlns");
    private static readonly XName PropFind = XName.Get("{DAV:}propfind");
    private static readonly XName Include = XName.Get("{DAV:}include");
    private static readonly XName Prop = XName.Get("{DAV:}prop");
    private static readonly XName AllProp = XName.Get("{DAV:}allprop");

    private static string BuildAllPropRequest(IReadOnlyCollection<XName> customProperties, IReadOnlyCollection<NamespaceAttr> namespaces)
    {
        var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
        var propFind = new XElement(
            PropFind,
            new XAttribute(XNamespace.Xmlns + "D", "DAV:"),
            new XElement(AllProp)
        );
        if (customProperties.Any())
        {
            var include = new XElement(Include);
            foreach (var ns in namespaces)
            {
                var nsAttr = string.IsNullOrEmpty(ns.Prefix) 
                    ? Xmlns
                    : XNamespace.Xmlns + ns.Prefix;
                include.SetAttributeValue(nsAttr, ns.Namespace);
            }
            foreach (var prop in customProperties)
            {
                include.Add(new XElement(prop));
            }
            propFind.Add(include);
        }
        doc.Add(propFind);
        return doc.ToStringWithDeclaration();
    }

    private static string BuildNamedPropRequest(IReadOnlyCollection<XName> customProperties, IEnumerable<NamespaceAttr> namespaces)
    {
        var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
        var propFind = new XElement(PropFind, new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
        if (customProperties.Any())
        {
            var propEl = new XElement(Prop);
            foreach (var ns in namespaces)
            {
                var nsAttr = string.IsNullOrEmpty(ns.Prefix) 
                    ? Xmlns
                    : XNamespace.Xmlns + ns.Prefix;

                propEl.SetAttributeValue(nsAttr, ns.Namespace);
            }
            foreach (var prop in customProperties)
            {
                propEl.Add(new XElement(prop));
            }
            propFind.Add(propEl);
        }
        doc.Add(propFind);
        return doc.ToStringWithDeclaration();
    }
}