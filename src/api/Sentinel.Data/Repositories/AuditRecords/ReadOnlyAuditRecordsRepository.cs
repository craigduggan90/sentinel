using Beans.Pageable;
using Microsoft.EntityFrameworkCore;
using Sentinel.Data.Context;
using Sentinel.Domain.Entities;

namespace Sentinel.Data.Repositories.AuditRecords;

/// <inheritdoc cref="IReadOnlyAuditRecordsRepository" />
public class ReadOnlyAuditRecordsRepository(ApiDbContext context)
    : RepositoryBase(context), IReadOnlyAuditRecordsRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AuditRecord>> GetAsync(
        string userId,
        int? offset = null,
        int? limit = null,
        CancellationToken cancellationToken = default) =>
        await Context.AuditRecords
            .Where(entity => entity.UserId == userId)
            .ApplyOffsetPagination(
                offset,
                limit ?? Constants.DefaultPageSize,
                queryable => queryable.OrderByDescending(entity => entity.Id))
            .ToListAsync(cancellationToken);
}