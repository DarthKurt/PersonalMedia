using System.Xml.Linq;

namespace PersonalMedia.WebDav.Client;

internal record PropStat(XElement Element, int StatusCode, string? Description);