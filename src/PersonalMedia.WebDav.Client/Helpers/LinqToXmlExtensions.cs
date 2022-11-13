using System.Xml.Linq;

namespace PersonalMedia.WebDav.Client.Helpers;

internal static class LinqToXmlExtensions
{
    public static string ToStringWithDeclaration(this XDocument doc)
        => doc.Declaration + Environment.NewLine + doc;

    public static XElement? LocalNameElement(this XElement parent, string localName)
        => LocalNameElement(parent, localName, StringComparison.Ordinal);

    public static XElement? LocalNameElement(this XElement parent, string localName, StringComparison comparisonType)
        => parent.Elements().FirstOrDefault(e => e.Name.LocalName.Equals(localName, comparisonType));

    public static IEnumerable<XElement> LocalNameElements(this XElement parent, string localName)
        => LocalNameElements(parent, localName, StringComparison.Ordinal);

    public static IEnumerable<XElement> LocalNameElements(this XElement parent, string localName, StringComparison comparisonType)
        => parent.Elements().Where(e => e.Name.LocalName.Equals(localName, comparisonType));

    public static string GetInnerXml(this XElement element)
    {
        using var reader = element.CreateReader();

        reader.MoveToContent();
        return reader.ReadInnerXml();
    }

    public static void SetInnerXml(this XElement element, string innerXml)
    {
        element.ReplaceNodes(XElement.Parse("<dummy>" + innerXml + "</dummy>").Nodes());
    }
}