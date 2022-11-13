using System.Xml.Linq;

namespace PersonalMedia.WebDav.Client.Helpers;

internal static class XDocumentHelper
{
    public static bool TryParse(string? text, out XDocument result)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            result = new XDocument();
            return false;
        }

        try
        {
            result = XDocument.Parse(text);
            return true;
        }
        catch
        {
            result = new XDocument();
            return false;
        }
    }
}
