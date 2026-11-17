using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Infrastructure.Errors.Handlers;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Api.Infrastructure.Swagger.Examples.V1.Common;

[ExcludeFromCodeCoverage]
public class UnhandledProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples() => UnhandledExceptionHandler.GetProblemDetails();
}