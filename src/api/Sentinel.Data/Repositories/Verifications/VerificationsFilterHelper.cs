using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.Repositories.Verifications;

public static class VerificationsFilterHelper
{
    public static IQueryable<Verification> ApplyUserIdFilter(this IQueryable<Verification> queryable, string value) =>
        queryable.Where(entity => entity.UserId == value);

    public static IQueryable<Verification> ApplyStatusFilter(
        this IQueryable<Verification> queryable,
        VerificationStatus? value) =>
        value is null
            ? queryable
            : queryable.Where(entity => entity.Status == value);

    public static IQueryable<Verification> ApplyMinimumAgeFilter(this IQueryable<Verification> queryable, int value) =>
        queryable.Where(entity => entity.Age >= value);

    public static IQueryable<Verification> ApplyTimestampFromFilter(this IQueryable<Verification> queryable, DateTime value) =>
        queryable.Where(entity => entity.Timestamp >= value);
}