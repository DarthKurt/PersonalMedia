using System.Text.RegularExpressions;
using System.Xml.Linq;
using PersonalMedia.WebDav.Client.Abstractions.Core;

namespace PersonalMedia.WebDav.Client.Helpers;

internal static class MultiStatusParser
{
    private static readonly Regex StatusCodeRegex = new(@".*(\d{3}).*");

    public static List<PropStat> GetPropStats(XElement xresponse) =>
        xresponse.LocalNameElements("propstat", StringComparison.OrdinalIgnoreCase)
            .Select(x => new PropStat(x, GetStatusCodeFromPropstat(x), GetDescriptionFromPropStat(x)))
            .ToList();

    public static List<WebDavPropertyStatus> GetPropertyStatuses(XElement response)
    {
        var propStats = GetPropStats(response);
        return GetPropertyStatuses(propStats);
    }

    public static List<WebDavPropertyStatus> GetPropertyStatuses(List<PropStat> propStats)
        => propStats
            .SelectMany(x => x.Element.LocalNameElements("prop", StringComparison.OrdinalIgnoreCase)
                .SelectMany(p => p.Elements())
                .Select(p => (Prop: p, x.StatusCode, x.Description)))
            .Select(x => new WebDavPropertyStatus(x.Prop.Name, x.StatusCode, x.Description))
            .ToList();

    public static List<XElement> GetProperties(List<PropStat> propStats)
        => propStats
            .Where(x => IsSuccessStatusCode(x.StatusCode))
            .SelectMany(x => x.Element.LocalNameElements("prop", StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.Elements())
            .ToList();

    private static bool IsSuccessStatusCode(int statusCode) => statusCode is >= 200 and <= 299;

    private static string? GetDescriptionFromPropStat(XElement propStat)
        => propStat.LocalNameElement("responsedescription", StringComparison.OrdinalIgnoreCase)?.Value
           ?? propStat.LocalNameElement("status", StringComparison.OrdinalIgnoreCase)?.Value;

    private static int GetStatusCodeFromPropstat(XElement propStat)
    {
        var statusRawValue = propStat.LocalNameElement("status", StringComparison.OrdinalIgnoreCase)?.Value;

        if (string.IsNullOrEmpty(statusRawValue))
            return default;

        var statusCodeGroup = StatusCodeRegex.Match(statusRawValue).Groups[1];
        if (!statusCodeGroup.Success)
            return default;

        return !int.TryParse(statusCodeGroup.Value, out var statusCode) ? default : statusCode;
    }
}