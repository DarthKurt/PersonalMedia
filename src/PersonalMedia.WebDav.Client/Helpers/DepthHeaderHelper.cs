using PersonalMedia.WebDav.Client.Abstractions.Core;

namespace PersonalMedia.WebDav.Client.Helpers;

internal static class DepthHeaderHelper
{
    public static string GetValueForPropfind(ApplyTo.Propfind applyTo) =>
        applyTo switch
        {
            ApplyTo.Propfind.ResourceOnly => "0",
            ApplyTo.Propfind.ResourceAndChildren => "1",
            ApplyTo.Propfind.ResourceAndAncestors => "infinity",
            _ => throw new ArgumentOutOfRangeException(nameof(applyTo))
        };

    public static string GetValueForCopy(ApplyTo.Copy applyTo) =>
        applyTo switch
        {
            ApplyTo.Copy.ResourceOnly => "0",
            ApplyTo.Copy.ResourceAndAncestors => "infinity",
            _ => throw new ArgumentOutOfRangeException(nameof(applyTo))
        };

    public static string GetValueForLock(ApplyTo.Lock applyTo) =>
        applyTo switch
        {
            ApplyTo.Lock.ResourceOnly => "0",
            ApplyTo.Lock.ResourceAndAncestors => "infinity",
            _ => throw new ArgumentOutOfRangeException(nameof(applyTo))
        };
}