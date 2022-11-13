using System.Net.Http.Headers;

namespace PersonalMedia.WebDav.Client.Services;

internal interface IWebDavHttpAdapter
{
    Uri? BaseAddress { get; }

    Task<HttpResponseMessage> SendAsync(
        Uri? requestUri,
        HttpMethod method,
        IReadOnlyCollection<KeyValuePair<string, string>> headers,
        HttpCompletionOption httpCompletionOption,
        CancellationToken cancellationToken);

    Task<HttpResponseMessage> SendAsync(
        Uri? requestUri,
        HttpMethod method,
        IReadOnlyCollection<KeyValuePair<string, string>> headers,
        HttpContent content,
        MediaTypeHeaderValue contentType,
        HttpCompletionOption httpCompletionOption,
        CancellationToken cancellationToken);
}