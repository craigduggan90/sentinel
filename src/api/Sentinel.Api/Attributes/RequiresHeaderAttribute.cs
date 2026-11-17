using Microsoft.AspNetCore.Mvc.Filters;
using Sentinel.Api.Exceptions;

namespace Sentinel.Api.Attributes;

/// <summary>Fails the request (428) unless the named header is present and non-blank.</summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequiresHeaderAttribute(string headerName) : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(headerName, out var values)
            || values.Count == 0
            || string.IsNullOrWhiteSpace(values[0]))
        {
            throw new MissingHeaderException(headerName);
        }
    }
}