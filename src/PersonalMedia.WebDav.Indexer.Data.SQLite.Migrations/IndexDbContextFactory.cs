using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PersonalMedia.WebDav.Indexer.Data.SQLite.Migrations;

public sealed class IndexDbContextFactory : IDesignTimeDbContextFactory<FilesIndexContext>
{
    public FilesIndexContext CreateDbContext(string[] args)
    {
        // prepare the configuration from arguments
        var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

        // pass configuration to the context builder
        var builder = new DbContextOptionsBuilder<FilesIndexContext>();
        var connectionString = configuration.GetConnectionString(nameof(FilesIndexContext));

        if (string.IsNullOrWhiteSpace(connectionString))
            return ThrowHelper.ThrowInvalidOperationException<FilesIndexContext>(
                "Connection string should not be empty");

        var options = builder
            .SetupContext(connectionString)
            .Options;

        // instantiate context
        return new FilesIndexContext(options);
    }
}