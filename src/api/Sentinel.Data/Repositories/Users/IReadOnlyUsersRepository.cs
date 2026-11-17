using Beans.Pageable;
using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.Repositories.Users;

/// <summary>Describes a read-only repository containing instances of <see cref="User"/>.</summary>
public interface IReadOnlyUsersRepository : IReadOnlyRepository<User>
{
    /// <summary>Get a <see cref="User"/>, with their scopes, by their IdP subject.</summary>
    /// <param name="remoteId">The user's IdP subject.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<User?> GetByRemoteIdAsync(string remoteId, CancellationToken cancellationToken);

    /// <summary>Get a <see cref="User"/> by their email address (case-insensitive).</summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>Get a page of users matching the given filters.</summary>
    /// <param name="type">Limit results to users of this type.</param>
    /// <param name="email">Limit results to those whose email address contains this value.</param>
    /// <param name="dateFilter">Limit results to those matching this date filter.</param>
    /// <param name="cursor">The cursor of the last item on the previous page, if any.</param>
    /// <param name="pageSize">The maximum number of results to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IReadOnlyCollection<User>> GetAsync(
        UserType type,
        string? email = null,
        DateFilter? dateFilter = null,
        long? cursor = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default);
}