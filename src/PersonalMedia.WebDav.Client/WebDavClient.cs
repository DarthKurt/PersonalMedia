using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using CommunityToolkit.Diagnostics;
using PersonalMedia.WebDav.Client.Abstractions;
using PersonalMedia.WebDav.Client.Abstractions.Core;
using PersonalMedia.WebDav.Client.Abstractions.Request;
using PersonalMedia.WebDav.Client.Abstractions.Response;
using PersonalMedia.WebDav.Client.Helpers;
using PersonalMedia.WebDav.Client.Services;

namespace PersonalMedia.WebDav.Client;

/// <summary>
/// <inheritdoc cref="IWebDavClient"/>
/// </summary>
internal sealed class WebDavClient : IWebDavClient
{
    private readonly IWebDavHttpAdapter _httpAdapter;
    private readonly IResponseParser<PropFindResponse> _propFindResponseParser;
    private readonly IResponseParser<PropPatchResponse> _propPatchResponseParser;
    private readonly IResponseParser<LockResponse> _lockResponseParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavClient"/> class.
    /// </summary>
    /// <param name="httpAdapter">Http adapter for WebDav requests.</param>
    /// <param name="propFindResponseParser">PropFind parser.</param>
    /// <param name="propPatchResponseParser">PropPatch parser.</param>
    /// <param name="lockResponseParser">Lock parser.</param>
    public WebDavClient(
        IWebDavHttpAdapter httpAdapter,
        IResponseParser<PropFindResponse> propFindResponseParser,
        IResponseParser<PropPatchResponse> propPatchResponseParser,
        IResponseParser<LockResponse> lockResponseParser
        )
    {
        _httpAdapter = httpAdapter;
        _propFindResponseParser = propFindResponseParser;
        _propPatchResponseParser = propPatchResponseParser;
        _lockResponseParser = lockResponseParser;
    }


    public Task<PropFindResponse> PropFind(string requestUri, CancellationToken cancellation)
        => PropFind(CreateUri(requestUri), new PropFindParameters(), cancellation);

    public Task<PropFindResponse> PropFind(Uri? requestUri, CancellationToken cancellation)
        => PropFind(requestUri, new PropFindParameters(), cancellation);


    public Task<PropFindResponse> PropFind(string requestUri, PropFindParameters parameters, CancellationToken cancellation)
        => PropFind(CreateUri(requestUri), parameters, cancellation);

    public async Task<PropFindResponse> PropFind(Uri? requestUri, PropFindParameters parameters, CancellationToken cancellation)
    {
        Guard.IsNotNullOrWhiteSpace(requestUri?.ToString(), nameof(requestUri));

        var applyTo = parameters.ApplyTo ?? ApplyTo.Propfind.ResourceAndChildren;
        var headers = new HeaderBuilder()
            .Add(WebDavHeaders.Depth, DepthHeaderHelper.GetValueForPropfind(applyTo))
            .AddWithOverwrite(parameters.Headers)
            .Build();

        HttpResponseMessage response;
        if (parameters.RequestType != PropfindRequestType.AllPropertiesImplied)
        {
            var content = PropfindRequestBuilder.BuildRequest(parameters.RequestType, parameters.CustomProperties, parameters.Namespaces);
            var requestBody = new StringContent(content);

            response = await _httpAdapter.SendAsync(
                    requestUri,
                    WebDavMethod.PropFind,
                    headers,
                    requestBody,
                    parameters.ContentType,
                    HttpCompletionOption.ResponseContentRead,
                    cancellation)
                .ConfigureAwait(false);
        }
        else
        {
            response = await _httpAdapter.SendAsync(
                    requestUri,
                    WebDavMethod.PropFind,
                    headers,
                    HttpCompletionOption.ResponseContentRead,
                    cancellation)
                .ConfigureAwait(false);
        }

        var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
        return _propFindResponseParser.Parse(responseContent, (int)response.StatusCode, response.ReasonPhrase);
    }

    public Task<PropPatchResponse> PropPatch(string requestUri, ProppatchParameters parameters)
        => PropPatch(CreateUri(requestUri), parameters);


    public async Task<PropPatchResponse> PropPatch(Uri? requestUri, ProppatchParameters parameters)
    {
        Guard.IsNotNullOrWhiteSpace(requestUri?.ToString(), nameof(requestUri));

        var headerBuilder = new HeaderBuilder();
        if (!string.IsNullOrEmpty(parameters.LockToken))
            headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken));

        var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
        var requestBody = ProppatchRequestBuilder.BuildRequestBody(
            parameters.PropertiesToSet,
            parameters.PropertiesToRemove,
            parameters.Namespaces);

        var content = new StringContent(requestBody);

        var response = await _httpAdapter.SendAsync(
                requestUri,
                WebDavMethod.PropPatch,
                headers,
                content,
                parameters.ContentType,
                HttpCompletionOption.ResponseContentRead,
                parameters.CancellationToken)
            .ConfigureAwait(false);

        var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
        return _propPatchResponseParser.Parse(responseContent, (int)response.StatusCode, response.ReasonPhrase);
    }


    public Task<WebDavResponse> Mkcol(string requestUri)
        => Mkcol(CreateUri(requestUri), new MkColParameters());


    public Task<WebDavResponse> Mkcol(Uri? requestUri)
        => Mkcol(requestUri, new MkColParameters());

    public Task<WebDavResponse> Mkcol(string requestUri, MkColParameters parameters)
        => Mkcol(CreateUri(requestUri), parameters);


    public async Task<WebDavResponse> Mkcol(Uri? requestUri, MkColParameters parameters)
    {
        Guard.IsNotNullOrWhiteSpace(requestUri?.ToString(), nameof(requestUri));

        var headerBuilder = new HeaderBuilder();
        if (!string.IsNullOrEmpty(parameters.LockToken))
            headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken));

        var headers = headerBuilder
            .AddWithOverwrite(parameters.Headers)
            .Build();

        var response = await _httpAdapter.SendAsync(
                requestUri,
                WebDavMethod.Mkcol,
                headers,
                HttpCompletionOption.ResponseContentRead,
                parameters.CancellationToken)
            .ConfigureAwait(false);

        return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
    }


    public Task<WebDavStreamResponse> GetRawFile(string requestUri)
        => GetFile(CreateUri(requestUri), false, new GetFileParameters());


    public Task<WebDavStreamResponse> GetRawFile(Uri? requestUri)
        => GetFile(requestUri, false, new GetFileParameters());


    public Task<WebDavStreamResponse> GetRawFile(string requestUri, GetFileParameters parameters)
        => GetFile(CreateUri(requestUri), false, parameters);


    public Task<WebDavStreamResponse> GetRawFile(Uri? requestUri, GetFileParameters parameters)
        => GetFile(requestUri, false, parameters);


    public Task<WebDavStreamResponse> GetProcessedFile(string requestUri)
        => GetFile(CreateUri(requestUri), true, new GetFileParameters());

    public Task<WebDavStreamResponse> GetProcessedFile(Uri? requestUri)
        => GetFile(requestUri, true, new GetFileParameters());


    public Task<WebDavStreamResponse> GetProcessedFile(string requestUri, GetFileParameters parameters)
        => GetFile(CreateUri(requestUri), true, parameters);

    public Task<WebDavStreamResponse> GetProcessedFile(Uri? requestUri, GetFileParameters parameters)
        => GetFile(requestUri, true, parameters);

    public async Task<WebDavStreamResponse> GetFile(
        Uri? requestUri, bool translate, GetFileParameters parameters)
    {
        var response = await GetFileResponse(requestUri, translate, parameters)
            .ConfigureAwait(false);
        var stream = await response.Content.ReadAsStreamAsync(parameters.CancellationToken)
            .ConfigureAwait(false);

        return new WebDavStreamResponse(response, stream);
    }

    public Task<HttpResponseMessage> GetFileResponse(Uri? requestUri, bool translate, GetFileParameters parameters)
    {
        Guard.IsNotNullOrWhiteSpace(requestUri?.ToString(), "requestUri");

        var headers = new HeaderBuilder()
            .Add(WebDavHeaders.Translate, translate ? "t" : "f")
            .AddWithOverwrite(parameters.Headers)
            .Build();

        return _httpAdapter.SendAsync(
                requestUri,
                HttpMethod.Get,
                headers,
                HttpCompletionOption.ResponseHeadersRead,
                parameters.CancellationToken);
    }

    public Task<WebDavResponse> Delete(string requestUri)
        => Delete(CreateUri(requestUri), new DeleteParameters());

    public Task<WebDavResponse> Delete(Uri? requestUri)
        => Delete(requestUri, new DeleteParameters());

    public Task<WebDavResponse> Delete(string requestUri, DeleteParameters parameters)
        => Delete(CreateUri(requestUri), parameters);

    public async Task<WebDavResponse> Delete(Uri? requestUri, DeleteParameters parameters)
    {
        Guard.IsNotNullOrWhiteSpace(requestUri?.ToString(), "requestUri");

        var headerBuilder = new HeaderBuilder();
        if (!string.IsNullOrEmpty(parameters.LockToken))
            headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken));

        var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();

        var response = await _httpAdapter.SendAsync(
                requestUri,
                HttpMethod.Get,
                headers,
                HttpCompletionOption.ResponseContentRead,
                parameters.CancellationToken)
            .ConfigureAwait(false);

        return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
    }

    public Task<WebDavResponse> PutFile(string requestUri, Stream stream)
        => PutFile(CreateUri(requestUri), new StreamContent(stream), new PutFileParameters());

    public Task<WebDavResponse> PutFile(Uri? requestUri, Stream stream)
        => PutFile(requestUri, new StreamContent(stream), new PutFileParameters());

    public Task<WebDavResponse> PutFile(string requestUri, Stream stream, string contentType)
    {
        var fileParameters = new PutFileParameters { ContentType = new MediaTypeHeaderValue(contentType) };
        return PutFile(CreateUri(requestUri), new StreamContent(stream), fileParameters);
    }

    public Task<WebDavResponse> PutFile(Uri? requestUri, Stream stream, string contentType)
    {
        var fileParameters = new PutFileParameters { ContentType = new MediaTypeHeaderValue(contentType) };
        return PutFile(requestUri, new StreamContent(stream), fileParameters);
    }


    public Task<WebDavResponse> PutFile(string requestUri, Stream stream, PutFileParameters parameters)
        => PutFile(CreateUri(requestUri), new StreamContent(stream), parameters);

    public Task<WebDavResponse> PutFile(Uri? requestUri, Stream stream, PutFileParameters parameters)
        => PutFile(requestUri, new StreamContent(stream), parameters);


    public Task<WebDavResponse> PutFile(string requestUri, HttpContent content)
        => PutFile(CreateUri(requestUri), content, new PutFileParameters());


    public Task<WebDavResponse> PutFile(Uri? requestUri, HttpContent content)
        => PutFile(requestUri, content, new PutFileParameters());


    public Task<WebDavResponse> PutFile(string requestUri, HttpContent content, PutFileParameters parameters)
        => PutFile(CreateUri(requestUri), content, parameters);

    public async Task<WebDavResponse> PutFile(Uri? requestUri, HttpContent content, PutFileParameters parameters)
    {
        Guard.IsNotNullOrWhiteSpace(requestUri?.ToString(), nameof(requestUri));
        Guard.IsNotNull(content);

        var headerBuilder = new HeaderBuilder();
        if (!string.IsNullOrEmpty(parameters.LockToken))
            headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken));

        var headers = headerBuilder
            .AddWithOverwrite(parameters.Headers)
            .Build();

        var response = await _httpAdapter.SendAsync(
                requestUri,
                HttpMethod.Put,
                headers,
                content,
                parameters.ContentType,
                HttpCompletionOption.ResponseContentRead,
                parameters.CancellationToken)
            .ConfigureAwait(false);

        return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
    }

    public Task<WebDavResponse> Copy(string sourceUri, string destUri)
        => Copy(CreateUri(sourceUri), CreateUri(destUri), new CopyParameters());


    public Task<WebDavResponse> Copy(Uri? sourceUri, Uri? destUri)
        => Copy(sourceUri, destUri, new CopyParameters());

    public Task<WebDavResponse> Copy(string sourceUri, string destUri, CopyParameters parameters)
        => Copy(CreateUri(sourceUri), CreateUri(destUri), parameters);

    public async Task<WebDavResponse> Copy(Uri? sourceUri, Uri? destUri, CopyParameters parameters)
    {
        Guard.IsNotNullOrWhiteSpace(sourceUri?.ToString(), nameof(sourceUri));
        Guard.IsNotNullOrWhiteSpace(destUri?.ToString(), nameof(destUri));

        var applyTo = parameters.ApplyTo ?? ApplyTo.Copy.ResourceAndAncestors;
        var headerBuilder = new HeaderBuilder()
            .Add(WebDavHeaders.Destination, GetAbsoluteUri(destUri))
            .Add(WebDavHeaders.Depth, DepthHeaderHelper.GetValueForCopy(applyTo))
            .Add(WebDavHeaders.Overwrite, parameters.Overwrite ? "T" : "F");

        if (!string.IsNullOrEmpty(parameters.DestLockToken))
            headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.DestLockToken));

        var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
        var response = await _httpAdapter.SendAsync(
                sourceUri,
                WebDavMethod.Copy,
                headers,
                HttpCompletionOption.ResponseContentRead,
                parameters.CancellationToken)
            .ConfigureAwait(false);

        return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
    }

    public Task<WebDavResponse> Move(string sourceUri, string destUri)
        => Move(CreateUri(sourceUri), CreateUri(destUri), new MoveParameters());

    public Task<WebDavResponse> Move(Uri? sourceUri, Uri? destUri)
        => Move(sourceUri, destUri, new MoveParameters());

    public Task<WebDavResponse> Move(string sourceUri, string destUri, MoveParameters parameters)
        => Move(CreateUri(sourceUri), CreateUri(destUri), parameters);

    public async Task<WebDavResponse> Move(Uri? sourceUri, Uri? destUri, MoveParameters parameters)
    {
        Guard.IsNotNullOrWhiteSpace(sourceUri?.ToString(), nameof(sourceUri));
        Guard.IsNotNullOrWhiteSpace(destUri?.ToString(), nameof(destUri));

        var headerBuilder = new HeaderBuilder()
            .Add(WebDavHeaders.Destination, GetAbsoluteUri(destUri))
            .Add(WebDavHeaders.Overwrite, parameters.Overwrite ? "T" : "F");

        if (!string.IsNullOrEmpty(parameters.SourceLockToken))
        {
            headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.SourceLockToken));
        }

        if (!string.IsNullOrEmpty(parameters.DestLockToken))
        {
            headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.DestLockToken));
        }

        var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
        var response = await _httpAdapter.SendAsync(
                sourceUri,
                WebDavMethod.Move,
                headers,
                HttpCompletionOption.ResponseContentRead,
                parameters.CancellationToken)
            .ConfigureAwait(false);

        return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
    }

    public Task<LockResponse> Lock(string requestUri)
        => Lock(CreateUri(requestUri), new LockParameters());

    public Task<LockResponse> Lock(Uri? requestUri)
        => Lock(requestUri, new LockParameters());

    public Task<LockResponse> Lock(string requestUri, LockParameters parameters)
        => Lock(CreateUri(requestUri), parameters);

    public async Task<LockResponse> Lock(Uri? requestUri, LockParameters parameters)
    {
        Guard.IsNotNullOrWhiteSpace(requestUri?.ToString(), nameof(requestUri));

        var headerBuilder = new HeaderBuilder();
        if (parameters.ApplyTo.HasValue)
        {
            headerBuilder.Add(WebDavHeaders.Depth, DepthHeaderHelper.GetValueForLock(parameters.ApplyTo.Value));
        }

        if (parameters.Timeout.HasValue)
        {
            headerBuilder.Add(WebDavHeaders.Timeout, $"Second-{parameters.Timeout.Value.TotalSeconds}");
        }

        var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
        var requestBody = LockRequestBuilder.BuildRequestBody(parameters);
        var content = new StringContent(requestBody);

        var response = await _httpAdapter.SendAsync(
                requestUri,
                WebDavMethod.Lock,
                headers,
                content,
                parameters.ContentType,
                HttpCompletionOption.ResponseContentRead,
                parameters.CancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            return new LockResponse((int)response.StatusCode, response.ReasonPhrase);

        var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
        return _lockResponseParser.Parse(responseContent, (int)response.StatusCode, response.ReasonPhrase);
    }

    public Task<WebDavResponse> Unlock(string requestUri, string lockToken)
        => Unlock(CreateUri(requestUri), new UnlockParameters(lockToken));

    public Task<WebDavResponse> Unlock(Uri requestUri, string lockToken)
        => Unlock(requestUri, new UnlockParameters(lockToken));

    public Task<WebDavResponse> Unlock(string requestUri, UnlockParameters parameters)
        => Unlock(CreateUri(requestUri), parameters);

    public async Task<WebDavResponse> Unlock(Uri? requestUri, UnlockParameters parameters)
    {
        Guard.IsNotNullOrWhiteSpace(requestUri?.ToString());

        var headers = new HeaderBuilder()
            .Add(WebDavHeaders.LockToken, $"<{parameters.LockToken}>")
            .AddWithOverwrite(parameters.Headers)
            .Build();

        var response = await _httpAdapter.SendAsync(
                requestUri,
                WebDavMethod.Unlock,
                headers,
                HttpCompletionOption.ResponseContentRead,
                parameters.CancellationToken)
            .ConfigureAwait(false);

        return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
    }

    public Task<PropFindResponse> Search(string requestUri, SearchParameters parameters, CancellationToken cancellation)
        => Search(CreateUri(requestUri), parameters, cancellation);

    public async Task<PropFindResponse> Search(Uri? requestUri, SearchParameters parameters, CancellationToken cancellation)
    {
        Guard.IsNotNullOrWhiteSpace(requestUri?.ToString(), nameof(requestUri));
        parameters.AssertParametersAreValid();

        var headers = new HeaderBuilder()
            .AddWithOverwrite(parameters.Headers)
            .Build();

        var query = SearchRequestBuilder.BuildRequestBody(parameters);
        var requestBody = new StringContent(query);
        var response = await _httpAdapter.SendAsync(
                requestUri,
                WebDavMethod.Search,
                headers,
                requestBody,
                new MediaTypeHeaderValue(MediaTypeNames.Text.Html),
                HttpCompletionOption.ResponseContentRead,
                cancellation)
            .ConfigureAwait(false);

        var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);

        return _propFindResponseParser.Parse(responseContent, (int)response.StatusCode, response.ReasonPhrase);
    }

    public async Task<WebDavCapabilitiesResponse> GetCapabilities(CancellationToken cancellation)
    {
        var uri = new Uri("/", UriKind.Relative);

        var headers = new HeaderBuilder().Build();
        var response = await _httpAdapter.SendAsync(
                uri,
                HttpMethod.Options,
                headers,
                HttpCompletionOption.ResponseContentRead,
                cancellation)
            .ConfigureAwait(false);

        var allow = response.Content.Headers.Allow
                .Select(o => new HttpMethod(o));

        return new WebDavCapabilitiesResponse((int)response.StatusCode, response.ReasonPhrase, allow);
    }

    private static Uri? CreateUri(string? requestUri) =>
        string.IsNullOrWhiteSpace(requestUri)
            ? null
            : new Uri(requestUri, UriKind.RelativeOrAbsolute);

    private static Encoding GetResponseEncoding(HttpContent content, Encoding fallbackEncoding)
    {
        if (content.Headers.ContentType?.CharSet == null)
            return fallbackEncoding;

        try
        {
            return Encoding.GetEncoding(content.Headers.ContentType.CharSet);
        }
        catch (ArgumentException)
        {
            return fallbackEncoding;
        }
    }

    private static async Task<string> ReadContentAsString(HttpContent content)
    {
        var data = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
        return GetResponseEncoding(content, Encoding.UTF8).GetString(data, 0, data.Length);
    }

    private string GetAbsoluteUri(Uri? uri)
    {
        if (uri == null)
            return _httpAdapter.BaseAddress == null
                ? ThrowHelper.ThrowInvalidOperationException<string>(InvalidUriMessage)
                : _httpAdapter.BaseAddress.AbsoluteUri;

        if (uri.IsAbsoluteUri)
            return uri.AbsoluteUri;

        if (_httpAdapter.BaseAddress == null)
            return ThrowHelper.ThrowInvalidOperationException<string>(InvalidUriMessage);

        var newUri = new Uri(_httpAdapter.BaseAddress, uri);

        return newUri.AbsoluteUri;
    }

    private const string InvalidUriMessage =
        "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.";
}