namespace Sentinel.Core.Models;

/// <summary>The user on whose behalf the current request is being made, as resolved by the authoriser.</summary>
/// <param name="Id">The Sentinel identifier of the user.</param>
/// <param name="DisplayName">The display name of the user.</param>
public record Actor(string Id, string DisplayName);