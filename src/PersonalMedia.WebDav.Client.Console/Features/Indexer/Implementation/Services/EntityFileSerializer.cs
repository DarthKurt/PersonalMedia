using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Options;

namespace PersonalMedia.WebDav.Client.Console.Features.Indexer.Implementation.Services;

internal sealed class EntityFileSerializer
{
    private readonly JsonSerializerOptions _serializerOptions;

    public EntityFileSerializer(IOptionsSnapshot<JsonSerializerOptions> serializerOptions)
    {
        _serializerOptions = serializerOptions.Get("Indexer");
    }

    public async Task SaveFileAsync<T>(T data, string folder, string name)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(folder))
        {
            ThrowHelper.ThrowArgumentException("Folder string should not be empty");
            return;
        }

        var folderPath = Path.GetFullPath(folder);

        var directoryInfo = new DirectoryInfo(folderPath);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        await using var createStream = File.Create(Path.Combine(folderPath, name));
        await JsonSerializer.SerializeAsync(createStream, data, _serializerOptions);
        await createStream.DisposeAsync();
    }
}
