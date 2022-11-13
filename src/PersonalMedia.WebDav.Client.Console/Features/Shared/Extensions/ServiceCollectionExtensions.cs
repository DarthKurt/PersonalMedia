using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PersonalMedia.WebDav.Client.Console.Features.Shared.Extensions;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Extension method that allows to configure options by convention with a class name.
    /// </summary>
    /// <typeparam name="TOptions">
    /// Options class.
    /// </typeparam>
    /// <param name="services">
    /// Provided service collection.
    /// </param>
    /// <param name="configuration">
    /// Provided configuration.
    /// </param>
    /// <param name="isOptional">Indicates, that the section is optional.
    /// If <c>false</c>, a validation error could be thrown.</param>
    /// <returns>
    /// Merged <see cref="IConfiguration"/> instance.
    /// </returns>
    /// <remarks>
    ///     If <paramref name="isOptional"/> is <c>false</c>
    ///     and no matching sub-section is found with the specified key, an exception is raised.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// There is no section with key <typeparamref name="TOptions"/>.
    /// </exception>
    public static IServiceCollection ConfigureBySection<TOptions>(
        this IServiceCollection services, IConfiguration configuration, bool isOptional = true)
        where TOptions : class
        => services.Configure<TOptions>(isOptional
            ? configuration.GetSection(typeof(TOptions).Name)
            : configuration.GetRequiredSection(typeof(TOptions).Name));
}