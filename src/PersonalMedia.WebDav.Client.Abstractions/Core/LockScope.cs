namespace PersonalMedia.WebDav.Client.Abstractions.Core;

/// <summary>
/// Specifies a type of lock - exclusive or shared.
/// </summary>
public enum LockScope
{
    Shared,
    Exclusive
}