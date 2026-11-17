using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Sentinel.Data.Context;

/// <summary>A factory for creating <see cref="ApiDbContext"/> instances.</summary>
/// <param name="configuration">The application configuration provider.</param>
public class ApiDbContextFactory(IConfiguration configuration) : IApiDbContextFactory
{
    /// <summary>Creates a new read-only <see cref="ApiDbContext"/> instance.</summary>
    /// <returns>A new context instance.</returns>
    /// <remarks>The caller is responsible for disposing the context; it will not be disposed by any dependency injection container.</remarks>
    public ApiDbContext CreateDbContext()
        => CreateDbContext(ContextType.ReadOnly);

    /// <inheritdoc />
    public ApiDbContext CreateDbContext(ContextType contextType)
    {
        var connectionStringName = contextType == ContextType.ReadWrite
            ? Constants.WriterConnectionStringName
            : Constants.ReaderConnectionStringName;

        var connectionString = configuration.GetConnectionString(connectionStringName)
                               ?? throw new ArgumentException(
                                   $"Connection string not available for {contextType} contexts.",
                                   nameof(contextType));

        var options = new DbContextOptionsBuilder<ApiDbContext>().UseSqlite(connectionString).Options;

        return new ApiDbContext(options);
    }
}