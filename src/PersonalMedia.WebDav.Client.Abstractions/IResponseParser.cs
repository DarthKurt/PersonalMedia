using PersonalMedia.WebDav.Client.Abstractions.Response;

namespace PersonalMedia.WebDav.Client.Abstractions;

public interface IResponseParser<out TResponse>
    where TResponse : WebDavResponse
{
    TResponse Parse(string response, int statusCode, string description);
}