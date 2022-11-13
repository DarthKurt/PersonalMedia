﻿namespace PersonalMedia.WebDav.Client.Helpers;

internal class HeaderBuilder
{
    private readonly List<KeyValuePair<string, string>> _headers;

    public HeaderBuilder()
    {
        _headers = new List<KeyValuePair<string, string>>();
    }

    public HeaderBuilder Add(string name, string value)
    {
        _headers.Add(new KeyValuePair<string, string>(name, value));
        return this;
    }

    public HeaderBuilder AddWithOverwrite(IReadOnlyCollection<KeyValuePair<string, string>> headers)
    {
        foreach (var header in headers)
        {
            _headers.RemoveAll(x => x.Key.Equals(header.Key, StringComparison.CurrentCultureIgnoreCase));
        }
        _headers.AddRange(headers);
        return this;
    }

    public List<KeyValuePair<string, string>> Build()
    {
        return _headers;
    }
}