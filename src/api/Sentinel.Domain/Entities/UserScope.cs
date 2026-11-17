using Sentinel.Domain.Exceptions;

namespace Sentinel.Domain.Entities;

public class UserScope(string userId, string scope)
{
    public string UserId { get; } = userId;

    public string Scope { get; } = scope;

    /// <summary>The user to whom the scope applies. </summary>
    /// <exception cref="NavigationPropertyException">when the user is not initialised.</exception>
    public User User
    {
        get => field ?? throw NavigationPropertyException.ForUninitialisedProperty(nameof(User));
        init;
    }
}