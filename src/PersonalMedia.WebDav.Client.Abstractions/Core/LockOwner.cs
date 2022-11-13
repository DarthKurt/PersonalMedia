namespace PersonalMedia.WebDav.Client.Abstractions.Core;

/// <summary>
/// Represents a lock owner.
/// </summary>
public abstract class LockOwner
{
    /// <summary>
    /// Gets a value representing an owner.
    /// </summary>
    public abstract string Value { get; }
}