using Beans.Pageable;
using Microsoft.EntityFrameworkCore;
using Sentinel.Data.Context;
using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.Repositories.Verifications;

/// <inheritdoc cref="IReadOnlyVerificationsRepository" />
public class ReadOnlyVerificationsRepository(ApiDbContext context)
    : RepositoryBase(context), IReadOnlyVerificationsRepository
{
    /// <inheritdoc />
    public async Task<Verification?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await Context.Verifications.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<Verification?> GetLatestAsync(
        string userId,
        int minimumAge,
        VerificationStatus status,
        DateTime since,
        CancellationToken cancellationToken) =>
        await Context.Verifications
            .ApplyUserIdFilter(userId)
            .ApplyStatusFilter(status)
            .ApplyMinimumAgeFilter(minimumAge)
            .ApplyTimestampFromFilter(since)
            .OrderByDescending(entity => entity.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Verification>> GetAsync(
        string userId,
        VerificationStatus? status = null,
        DateFilter? dateFilter = null,
        long? cursor = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default) =>
        await Context.Verifications
            .ApplyUserIdFilter(userId)
            .ApplyStatusFilter(status)
            .ApplyDateFilter(dateFilter)
            .ApplyCursorPagination(cursor, pageSize ?? Constants.DefaultPageSize, SortOrder.Descending)
            .ToListAsync(cancellationToken);
}