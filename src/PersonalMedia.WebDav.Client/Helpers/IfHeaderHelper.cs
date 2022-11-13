namespace PersonalMedia.WebDav.Client.Helpers;

internal static class IfHeaderHelper
{
    public static string GetHeaderValue(string lockToken)
    {
        return $"(<{lockToken}>)";
    }
}