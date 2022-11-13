namespace PersonalMedia.WebDav.Client.Abstractions.Core;

public static class WebDavMethod
{
    public static readonly HttpMethod PropFind = new("PROPFIND");

    public static readonly HttpMethod PropPatch = new("PROPPATCH");

    public static readonly HttpMethod Mkcol = new("MKCOL");

    public static readonly HttpMethod Copy = new("COPY");

    public static readonly HttpMethod Move = new("MOVE");

    public static readonly HttpMethod Lock = new("LOCK");

    public static readonly HttpMethod Unlock = new("UNLOCK");

    public static readonly HttpMethod Search = new("SEARCH");
    public static readonly HttpMethod Report = new("REPORT");
}