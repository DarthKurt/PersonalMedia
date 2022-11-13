using System.Xml.Linq;
using CommunityToolkit.Diagnostics;

namespace PersonalMedia.WebDav.Client.Abstractions.Core;

/// <summary>
/// Represents a status of an operation on a resource property.
/// </summary>
public class WebDavPropertyStatus
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPropertyStatus"/> class.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="statusCode">The status code of the operation.</param>
    public WebDavPropertyStatus(XName name, int statusCode)
        : this(name, statusCode, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPropertyStatus"/> class.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="statusCode">The status code of the operation.</param>
    /// <param name="description">The description of the operation.</param>
    public WebDavPropertyStatus(XName? name, int statusCode, string? description)
    {
        Guard.IsNotNullOrEmpty(name?.ToString(), "name");

        Name = name;
        StatusCode = statusCode;
        Description = description;
    }

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public XName Name { get; }

    /// <summary>
    /// Gets the status code of the operation.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the description of the operation.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets a value indicating whether the operation on the property was successful.
    /// </summary>
    /// <value>
    /// <c>true</c> if the operation was successful; otherwise, <c>false</c>.
    /// </value>
    public virtual bool IsSuccessful => StatusCode is >= 200 and <= 299;

    public override string ToString()
    {
        return $"{{ Name: {Name}, StatusCode: {StatusCode}, Description: {Description} }}";
    }
}