using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalMedia.WebDav.Client.Console;
using PersonalMedia.WebDav.Client.Console.Configuration.Extensions;

// TODO: Migrate to System.CommandLine
// https://learn.microsoft.com/en-us/dotnet/standard/commandline/syntax?source=recommendations

using var app = new CommandLineApplication<Tool>();
var assembly = typeof(Tool).Assembly.GetName();
app.Name = assembly.Name;
app.Description = "CLI Tool for WebDav server.";
app.HelpOption();
app.VersionOption("-v|--version", assembly.Version?.ToString() ?? "DEBUG");

var configurationBuilder = new ConfigurationBuilder();

configurationBuilder
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", true);

var serviceProvider = new ServiceCollection()
    .ConfigureServices(configurationBuilder.Build())
    .BuildServiceProvider(true);

app.Conventions
    .UseDefaultConventions()
    .UseConstructorInjection(serviceProvider);

var result = await app.ExecuteAsync(args);

return result;
