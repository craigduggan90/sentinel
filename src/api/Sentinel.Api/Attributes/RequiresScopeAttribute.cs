using Microsoft.AspNetCore.Mvc.Filters;
using Sentinel.Api.Exceptions;
using Sentinel.Common;

namespace Sentinel.Api.Attributes;

/// <summary>Fails the request (403) unless the comma-delimited Scopes header contains the given scope.</summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequiresScopeAttribute(string scope) : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(Constants.ScopeHeaderKey, out var values)
            || !values.ToString()
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Contains(scope, StringComparer.OrdinalIgnoreCase))
        {
            throw new MissingScopeException(scope);
        }
    }
}