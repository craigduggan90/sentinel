namespace Sentinel.Api.Exceptions;

/// <summary>Represents an error arising when a required request header is missing or blank.</summary>
/// <param name="headerName">The name of the missing header.</param>
public class MissingHeaderException(string headerName) : Exception($"'{headerName}' header value is required.")
{
    /// <summary>The name of the missing header.</summary>
    public string HeaderName { get; } = headerName;
}