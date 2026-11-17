using Sentinel.Api.Exceptions;
using Sentinel.Common;
using Sentinel.Core.Models;
using Sentinel.Core.Services;

namespace Sentinel.Api.Services;

/// <summary>Builds the current <see cref="Actor"/> from the headers the gateway sets after authorisation.</summary>
public class ActorAccessor(IHttpContextAccessor contextAccessor) : IActorAccessor
{
    public Actor Current => field ??= BuildActor();

    private Actor BuildActor()
    {
        var httpContext = contextAccessor.HttpContext
                          ?? throw new InvalidOperationException("No active HTTP context is available.");

        return new Actor(
            GetRequiredHeader(httpContext, Constants.UserIdHeaderKey),
            GetRequiredHeader(httpContext, Constants.UserNameHeaderKey));
    }

    private static string GetRequiredHeader(HttpContext httpContext, string headerName)
    {
        if (!httpContext.Request.Headers.TryGetValue(headerName, out var values)
            || values.Count == 0
            || string.IsNullOrWhiteSpace(values[0]))
        {
            throw new MissingHeaderException(headerName);
        }

        return values[0]!;
    }
}