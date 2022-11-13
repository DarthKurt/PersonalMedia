using CommunityToolkit.Diagnostics;
using System.Xml.Linq;

namespace PersonalMedia.WebDav.Client.Abstractions.Core;

/// <summary>
/// Represents a WebDAV resource property.
/// </summary>
public class WebDavProperty
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavProperty"/> class.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    public WebDavProperty(XName? name, string value)
    {
        Guard.IsNotNullOrEmpty(name?.ToString());

        Name = name;
        Value = value;
    }

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public XName Name { get; }

    /// <summary>
    /// Gets the property value.
    /// </summary>
    public string Value { get; }

    public override string ToString()
    {
        return $"{{ Name: {Name}, Value: {Value} }}";
    }
}