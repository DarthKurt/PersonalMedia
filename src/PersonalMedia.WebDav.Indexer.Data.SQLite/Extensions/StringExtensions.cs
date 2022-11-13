using System.Text.RegularExpressions;

namespace PersonalMedia.WebDav.Indexer.Data.SQLite.Extensions;

public static class StringExtensions
{
    private static readonly Regex HtmlRegex = new("<[^>]*>", RegexOptions.Compiled);

    public static string NormalizeText(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Remove html tags
        text = HtmlRegex.Replace(text, string.Empty);

        // TODO: add other normalizers here, such as `remove diacritics`, `fix Persian Ye-Ke` and so on ...

        return text;
    }
}