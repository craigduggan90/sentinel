using Beans.Pageable;
using Microsoft.EntityFrameworkCore;
using Sentinel.Data.Context;
using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.Repositories.Users;

/// <inheritdoc cref="IReadOnlyUsersRepository" />
public class ReadOnlyUsersRepository(ApiDbContext context)
    : RepositoryBase(context), IReadOnlyUsersRepository
{
    /// <inheritdoc />
    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await Context.Users.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<User?> GetByRemoteIdAsync(string remoteId, CancellationToken cancellationToken) =>
        await Context.Users
            .Include(entity => entity.Scopes)
            .SingleOrDefaultAsync(entity => entity.RemoteId == remoteId, cancellationToken);

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        await Context.Users.SingleOrDefaultAsync(entity => entity.Email.ToLower() == email.ToLower(), cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<User>> GetAsync(
        UserType type,
        string? email = null,
        DateFilter? dateFilter = null,
        long? cursor = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default) =>
        await Context.Users
            .ApplyTypeFilter(type)
            .ApplyEmailFilter(email)
            .ApplyDateFilter(dateFilter)
            .ApplyCursorPagination(cursor, pageSize ?? Constants.DefaultPageSize)
            .ToListAsync(cancellationToken);
}