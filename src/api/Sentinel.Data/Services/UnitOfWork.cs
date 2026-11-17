using Sentinel.Data.Context;
using Sentinel.Data.Repositories.AuditRecords;
using Sentinel.Data.Repositories.Users;
using Sentinel.Data.Repositories.Verifications;

namespace Sentinel.Data.Services;

/// <inheritdoc cref="IUnitOfWork" />
/// <param name="factory">The database context factory.</param>
/// <remarks>
/// As this service is responsible for managing the database contexts that it initializes, we implement the disposable
/// interfaces to ensure those contexts are disposed with the request scope.
/// </remarks>
public class UnitOfWork(IApiDbContextFactory factory) : IUnitOfWork, IDisposable, IAsyncDisposable
{
    private ApiDbContext? _context;

    /// <inheritdoc />
    public ApiDbContext Context => _context ??= factory.CreateDbContext(ContextType.ReadWrite);

    /// <inheritdoc />
    public IUsersRepository Users => field ??= new UsersRepository(Context);

    /// <inheritdoc />
    public IVerificationsRepository Verifications => field ??= new VerificationsRepository(Context);

    /// <inheritdoc />
    public IAuditRecordsRepository AuditRecords => field ??= new AuditRecordsRepository(Context);

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await Context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_context != null)
            await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}