using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Exceptions;
using Sentinel.Api.Infrastructure.Errors.Handlers;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Api.Infrastructure.Swagger.Examples.V1.Common;

[ExcludeFromCodeCoverage]
public class MissingScopeProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples() =>
        MissingScopeExceptionHandler.GetProblemDetails(new MissingScopeException(Scopes.Users.Read));
}