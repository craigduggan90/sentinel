namespace Sentinel.Common;

public static class Constants
{
    public const string IdempotencyHeaderKey = "Idempotency-Key";
    public const string IfMatchHeaderKey = "If-Match";
    public const string ETagHeaderKey = "ETag";
    public const string ApiVersionHeaderKey = "Api-Version";
    public const string ScopeHeaderKey = "Scopes";

    /// <summary>Set by the gateway from the authoriser's resolved user - never by the client.</summary>
    public const string UserIdHeaderKey = "Sentinel-User-Id";

    /// <summary>Set by the gateway from the authoriser's resolved user - never by the client.</summary>
    public const string UserNameHeaderKey = "Sentinel-User-Name";
}