using PersonalMedia.WebDav.Client.Abstractions;
using PersonalMedia.WebDav.Client.Abstractions.Response;
using PersonalMedia.WebDav.Client.Helpers;

namespace PersonalMedia.WebDav.Client.Services;

internal class PropPatchResponseParser : IResponseParser<PropPatchResponse>
{
    public PropPatchResponse Parse(string? response, int statusCode, string description)
    {
        if (!XDocumentHelper.TryParse(response, out var document)
            || document.Root == null)
        {
            return new PropPatchResponse(statusCode, description);
        }

        var propStatuses = document.Root.LocalNameElements("response", StringComparison.OrdinalIgnoreCase)
            .SelectMany(MultiStatusParser.GetPropertyStatuses)
            .ToList();
        return new PropPatchResponse(statusCode, description, propStatuses);
    }
}