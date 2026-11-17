using Sentinel.Data.Context;
using Sentinel.Data.Repositories.AuditRecords;
using Sentinel.Data.Repositories.Users;
using Sentinel.Data.Repositories.Verifications;
using Sentinel.Domain.Entities;

namespace Sentinel.Data.Services;

/// <summary>
/// Context within which read/write operations may be performed. All repositories accessed within a unit of work share
/// its (read-write) database context.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>The read-write database context for this unit of work.</summary>
    ApiDbContext Context { get; }

    /// <summary>Accessor for the <see cref="User"/> repository.</summary>
    IUsersRepository Users { get; }

    /// <summary>Accessor for the <see cref="Verification"/> repository.</summary>
    IVerificationsRepository Verifications { get; }

    /// <summary>Accessor for the <see cref="AuditRecord"/> repository.</summary>
    IAuditRecordsRepository AuditRecords { get; }

    /// <summary>Saves all changes made in this context to the database.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}