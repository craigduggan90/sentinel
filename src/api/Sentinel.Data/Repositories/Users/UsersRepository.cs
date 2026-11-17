using Sentinel.Data.Context;
using Sentinel.Domain.Entities;

namespace Sentinel.Data.Repositories.Users;

/// <inheritdoc cref="IUsersRepository" />
public class UsersRepository(ApiDbContext context)
    : ReadOnlyUsersRepository(context), IUsersRepository
{
    /// <inheritdoc />
    public async Task<User> CreateAsync(User entity, CancellationToken cancellationToken)
    {
        await Context.Users.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public Task<User> UpdateAsync(User entity, CancellationToken cancellationToken)
    {
        Context.Users.Update(entity);
        return Task.FromResult(entity);
    }

    /// <inheritdoc />
    public Task<User> DeleteAsync(User entity, CancellationToken cancellationToken)
    {
        Context.Users.Remove(entity);
        return Task.FromResult(entity);
    }
}