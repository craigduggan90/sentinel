using Microsoft.AspNetCore.Mvc.Filters;
using Sentinel.Core.Services;

namespace Sentinel.Api.Attributes;

/// <summary>Fails the request up-front (428) unless the gateway has supplied the actor headers.</summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequiresCurrentActorAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var actorAccessor = context.HttpContext.RequestServices.GetRequiredService<IActorAccessor>();
        _ = actorAccessor.Current;
    }
}