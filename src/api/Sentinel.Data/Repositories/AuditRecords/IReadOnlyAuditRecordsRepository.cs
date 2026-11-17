using Sentinel.Domain.Entities;

namespace Sentinel.Data.Repositories.AuditRecords;

/// <summary>Describes a read-only repository containing instances of <see cref="AuditRecord"/>.</summary>
public interface IReadOnlyAuditRecordsRepository
{
    /// <summary>Get a page of a user's audit records, newest first.</summary>
    /// <param name="userId">The user's identifier.</param>
    /// <param name="offset">The number of records to skip.</param>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IReadOnlyCollection<AuditRecord>> GetAsync(
        string userId,
        int? offset = null,
        int? limit = null,
        CancellationToken cancellationToken = default);
}