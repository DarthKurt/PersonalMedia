using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PersonalMedia.WebDav.Client.Console.Features.Shared.Extensions;
using PersonalMedia.WebDav.Client.Console.Features.TestConnection.Abstractions.Services;
using PersonalMedia.WebDav.Client.Console.Features.TestConnection.Configuration;
using PersonalMedia.WebDav.Client.Console.Features.TestConnection.Implementation.Services;

namespace PersonalMedia.WebDav.Client.Console.Features.TestConnection;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTestConnectionFeature(
        this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        return serviceCollection
            .ConfigureBySection<ConnectionTesterSettings>(configuration)
            .AddTransient<IPostConfigureOptions<ConnectionTesterSettings>, ConnectionTesterSettingsConfigurator>()
            .AddTransient<IConnectionTester, ConnectionTester>();
    }
}