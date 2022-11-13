using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonalMedia.WebDav.Client.Console.Features.Indexer;
using PersonalMedia.WebDav.Client.Console.Features.TestConnection;
using PersonalMedia.WebDav.Client.Extensions;

namespace PersonalMedia.WebDav.Client.Console.Configuration.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(
        this IServiceCollection serviceCollection, IConfiguration configuration)
    {


        return serviceCollection
            .AddIndexerFeature(configuration)
            .AddTestConnectionFeature(configuration)
            .AddSingleton(PhysicalConsole.Singleton)
            .AddSingleton(configuration)
            .AddTransient<IConfigureOptions<WebDavClientParams>, WebDavClientParamsConfigurator>()
            .AddWebDavClient()
            .AddLogging(builder =>
                builder
                    .ClearProviders()
                    .AddConsole()
                    .AddConfiguration(configuration.GetSection("Logging")));
    }
}