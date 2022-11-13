using Microsoft.EntityFrameworkCore;

namespace PersonalMedia.WebDav.Indexer.Data.SQLite.Migrations;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder<T> SetupContext<T>(
        this DbContextOptionsBuilder<T> ctxOptionsBuilder, string connectionString)
        where T : DbContext
    {
        return ctxOptionsBuilder.UseSqlite(connectionString, optionsBuilder =>
        {
            var name = typeof(DbContextOptionsBuilderExtensions).Assembly.FullName;

            optionsBuilder.MigrationsAssembly(name);
        });
    }
}