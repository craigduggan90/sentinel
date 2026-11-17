using Sentinel.Data.Context;

namespace Sentinel.Api.IntegrationTests.TestHelpers;

/// <summary>Hands out the request's own scoped context, so reads and writes share the in-memory database.</summary>
public class TestApiDbContextFactory(ApiDbContext context) : IApiDbContextFactory
{
    public ApiDbContext CreateDbContext() => context;

    public ApiDbContext CreateDbContext(ContextType contextType) => context;
}