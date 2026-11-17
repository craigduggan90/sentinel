namespace Sentinel.Domain.Exceptions;

/// <summary>Represents an error arising from accessing a navigation property which has not been loaded.</summary>
/// <remarks>
/// Thrown from navigation property getters (e.g. <c>get => field ?? throw ...</c>) so that a query missing an
/// <c>Include</c> fails loudly, naming the property, rather than surfacing as a null further downstream.
/// </remarks>
public class NavigationPropertyException : Exception
{
    private NavigationPropertyException(string message, string propertyName)
        : base(message)
        => PropertyName = propertyName;

    /// <summary>The name of the navigation property.</summary>
    public string PropertyName { get; }

    /// <summary>Represents an error arising from accessing a navigation property which has not been loaded.</summary>
    /// <param name="propertyName">The name of the uninitialised navigation property.</param>
    public static NavigationPropertyException ForUninitialisedProperty(string propertyName)
        => new($"Uninitialised Navigation Property: '{propertyName}'.", propertyName);
}