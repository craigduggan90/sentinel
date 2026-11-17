using Beans.Pageable;
using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.Repositories.Verifications;

/// <summary>Describes a read-only repository containing instances of <see cref="Verification"/>.</summary>
public interface IReadOnlyVerificationsRepository : IReadOnlyRepository<Verification>
{
    /// <summary>Get the most recent verification for a user with the given status, at or above an age.</summary>
    /// <param name="userId">The user's identifier.</param>
    /// <param name="minimumAge">The minimum challenge age.</param>
    /// <param name="status">The verification status to match.</param>
    /// <param name="since">Only consider verifications which reached their status at or after this time (UTC).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<Verification?> GetLatestAsync(
        string userId,
        int minimumAge,
        VerificationStatus status,
        DateTime since,
        CancellationToken cancellationToken);

    /// <summary>Get a page of a user's verifications, newest first.</summary>
    /// <param name="userId">The user's identifier.</param>
    /// <param name="status">Limit results to verifications with this status.</param>
    /// <param name="dateFilter">Limit results to those matching this date filter.</param>
    /// <param name="cursor">The cursor of the last item on the previous page, if any.</param>
    /// <param name="pageSize">The maximum number of results to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IReadOnlyCollection<Verification>> GetAsync(
        string userId,
        VerificationStatus? status = null,
        DateFilter? dateFilter = null,
        long? cursor = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default);
}