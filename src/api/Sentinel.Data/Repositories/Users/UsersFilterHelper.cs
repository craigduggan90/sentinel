using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.Repositories.Users;

public static class UsersFilterHelper
{
    public static IQueryable<User> ApplyTypeFilter(this IQueryable<User> queryable, UserType value) =>
        queryable.Where(entity => entity.Type == value);

    public static IQueryable<User> ApplyEmailFilter(this IQueryable<User> queryable, string? value) =>
        value is null
            ? queryable
            : queryable.Where(entity => entity.Email.ToLower().Contains(value.ToLower()));
}