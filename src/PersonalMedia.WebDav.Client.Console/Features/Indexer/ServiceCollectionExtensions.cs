using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalMedia.WebDav.Client.Console.Features.Indexer.Abstractions.Services;
using PersonalMedia.WebDav.Client.Console.Features.Indexer.Implementation.Services;

namespace PersonalMedia.WebDav.Client.Console.Features.Indexer;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIndexerFeature(
        this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        return serviceCollection
            .AddTransient<EntityFileSerializer>()
            .AddTransient<IMetaInfoLoader, MetaInfoLoader>()
            .AddScoped<IFilesIndexer, FilesIndexer>()
            .Configure<JsonSerializerOptions>("Indexer",options =>
            {
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                options.WriteIndented = true;
                options.AllowTrailingCommas = false;
                options.PropertyNamingPolicy = null;
            });
    }
}
