using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PersonalMedia.WebDav.Client.Abstractions;
using PersonalMedia.WebDav.Client.Abstractions.Response;
using PersonalMedia.WebDav.Client.Services;

namespace PersonalMedia.WebDav.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebDavClient(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddHttpClient<IWebDavHttpAdapter, WebDavHttpAdapter>()
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<WebDavClientParams>>().Value;

                return new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    PreAuthenticate = config.PreAuthenticate,
                    UseDefaultCredentials = config.UseDefaultCredentials,
                    Credentials = config.Credentials,
                    UseProxy = config.UseProxy,
                    Proxy = config.Proxy
                };
            })
            .ConfigureHttpClient((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<WebDavClientParams>>().Value;

                client.BaseAddress = config.BaseAddress;

                if (config.Timeout.HasValue)
                {
                    client.Timeout = config.Timeout.Value;
                }

                foreach (var header in config.DefaultRequestHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            });

        return serviceCollection
            .AddTransient<IResponseParser<LockResponse>, LockResponseParser>()
            .AddTransient<IResponseParser<PropFindResponse>, PropFindResponseParser>()
            .AddTransient<IResponseParser<PropPatchResponse>, PropPatchResponseParser>()
            .AddTransient<IWebDavClient, WebDavClient>();
    }
}