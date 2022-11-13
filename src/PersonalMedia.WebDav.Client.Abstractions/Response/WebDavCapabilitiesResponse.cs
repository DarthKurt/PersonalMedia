namespace PersonalMedia.WebDav.Client.Abstractions.Response;

/// <summary>
/// Represents a response containing a WebDAV operations.
/// </summary>
public class WebDavCapabilitiesResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavCapabilitiesResponse"/> class.
    /// </summary>
    /// <param name="statusCode">The status code of the response.</param>
    /// <param name="description">The description of the response.</param>
    /// <param name="capabilities">List of <see cref="HttpMethod"/> operations.</param>
    public WebDavCapabilitiesResponse(int statusCode, string? description,  IEnumerable<HttpMethod> capabilities)
    {
        StatusCode = statusCode;
        Description = description;
        Capabilities = capabilities as List<HttpMethod> ?? capabilities.ToList();
    }

    /// <summary>
    /// Gets the status code of the response.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the description of the response.
    /// </summary>
    public string? Description { get; }

    public IEnumerable<HttpMethod> Capabilities { get; }

    public override string ToString()
        => $"OPTIONS WebDAV response - StatusCode: {StatusCode}, Description: {Description}";
}