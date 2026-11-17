using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Sentinel.Data.Context;

namespace Sentinel.Data.IntegrationTests.TestHelpers;

/// <summary>
/// Gives each test its own in-memory SQLite database, built by applying the real migrations so the schema under test
/// is the one that ships.
/// </summary>
public abstract class DatabaseAwareTestBase : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Filename=:memory:");

    /// <summary>The context used to seed and inspect the database.</summary>
    protected ApiDbContext Context { get; private set; } = null!;

    /// <summary>Creates a fresh context against the same database - useful to read back without tracked entities.</summary>
    protected ApiDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApiDbContext>().UseSqlite(_connection).Options);

    public virtual async ValueTask InitializeAsync()
    {
        await _connection.OpenAsync(TestContext.Current.CancellationToken);
        Context = CreateContext();
        await Context.Database.MigrateAsync(TestContext.Current.CancellationToken);
    }

    public virtual async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}