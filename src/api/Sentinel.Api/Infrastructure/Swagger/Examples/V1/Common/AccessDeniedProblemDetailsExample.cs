using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Infrastructure.Errors.Handlers;
using Sentinel.Core.Exceptions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Api.Infrastructure.Swagger.Examples.V1.Common;

[ExcludeFromCodeCoverage]
public class AccessDeniedProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples() =>
        AccessDeniedExceptionHandler.GetProblemDetails(new AccessDeniedException());
}