using Sentinel.Data.Context;
using Sentinel.Domain.Entities;

namespace Sentinel.Data.Repositories.AuditRecords;

/// <inheritdoc cref="IAuditRecordsRepository" />
public class AuditRecordsRepository(ApiDbContext context)
    : ReadOnlyAuditRecordsRepository(context), IAuditRecordsRepository
{
    /// <inheritdoc />
    public async Task<AuditRecord> CreateAsync(AuditRecord entity, CancellationToken cancellationToken)
    {
        await Context.AuditRecords.AddAsync(entity, cancellationToken);
        return entity;
    }
}