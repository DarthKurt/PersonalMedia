namespace PersonalMedia.WebDav.Client.Abstractions.Response;

/// <summary>
/// Represents a response of a WebDAV operation.
/// </summary>
public class WebDavResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavResponse"/> class.
    /// </summary>
    /// <param name="statusCode">The status code of the response.</param>
    /// <param name="description">The description of the response.</param>
    public WebDavResponse(int statusCode, string? description = null)
    {
        StatusCode = statusCode;
        Description = description;
    }

    /// <summary>
    /// Gets the status code of the response.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the description of the response.
    /// </summary>
    public string? Description { get; }

    public override string ToString()
        => $"WebDAV response - StatusCode: {StatusCode}, Description: {Description}";
}