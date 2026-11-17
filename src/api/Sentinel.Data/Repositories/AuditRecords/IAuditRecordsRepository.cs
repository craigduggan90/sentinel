using Sentinel.Domain.Entities;

namespace Sentinel.Data.Repositories.AuditRecords;

/// <summary>Describes an append-only repository containing instances of <see cref="AuditRecord"/>.</summary>
/// <remarks>The audit trail is never updated or deleted, so there's deliberately no update or delete here.</remarks>
public interface IAuditRecordsRepository : IReadOnlyAuditRecordsRepository
{
    /// <summary>Adds an <see cref="AuditRecord"/> to the current database context.</summary>
    /// <param name="entity">The record to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>This method does not commit changes to the database.</remarks>
    Task<AuditRecord> CreateAsync(AuditRecord entity, CancellationToken cancellationToken);
}