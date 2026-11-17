namespace Sentinel.Api.Exceptions;

/// <summary>Represents an error arising when the request does not carry the scope(s) an endpoint requires.</summary>
/// <param name="scopes">The required scopes.</param>
public class MissingScopeException(params string[] scopes) : Exception(scopes.Length == 1
    ? $"Required scope was not present: '{scopes.Single()}'."
    : $"Required scopes were not present: {string.Join(", ", scopes.Select(scope => $"'{scope}'"))}.")
{
    /// <summary>The required scopes.</summary>
    public IReadOnlyList<string> Scopes { get; } = scopes;
}