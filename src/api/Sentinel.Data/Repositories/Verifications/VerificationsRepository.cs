using Sentinel.Data.Context;
using Sentinel.Domain.Entities;

namespace Sentinel.Data.Repositories.Verifications;

/// <inheritdoc cref="IVerificationsRepository" />
public class VerificationsRepository(ApiDbContext context)
    : ReadOnlyVerificationsRepository(context), IVerificationsRepository
{
    /// <inheritdoc />
    public async Task<Verification> CreateAsync(Verification entity, CancellationToken cancellationToken)
    {
        await Context.Verifications.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public Task<Verification> UpdateAsync(Verification entity, CancellationToken cancellationToken)
    {
        Context.Verifications.Update(entity);
        return Task.FromResult(entity);
    }

    /// <inheritdoc />
    public Task<Verification> DeleteAsync(Verification entity, CancellationToken cancellationToken)
    {
        Context.Verifications.Remove(entity);
        return Task.FromResult(entity);
    }
}