using System.Net.Http.Headers;

namespace PersonalMedia.WebDav.Client.Services;

internal class WebDavHttpAdapter : IWebDavHttpAdapter
{
    private readonly HttpClient _httpClient;

    public WebDavHttpAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Uri? BaseAddress => _httpClient.BaseAddress;

    public async Task<HttpResponseMessage> SendAsync(
        Uri? requestUri,
        HttpMethod method,
        IReadOnlyCollection<KeyValuePair<string, string>> headers,
        HttpCompletionOption httpCompletionOption,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, requestUri);

        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        return await _httpClient.SendAsync(request, httpCompletionOption, cancellationToken);
    }

    public async Task<HttpResponseMessage> SendAsync(
        Uri? requestUri,
        HttpMethod method,
        IReadOnlyCollection<KeyValuePair<string, string>> headers,
        HttpContent content,
        MediaTypeHeaderValue contentType,
        HttpCompletionOption httpCompletionOption,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, requestUri);

        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        request.Content = content;
        request.Content.Headers.ContentType = contentType;

        return await _httpClient.SendAsync(request, httpCompletionOption, cancellationToken);
    }
}