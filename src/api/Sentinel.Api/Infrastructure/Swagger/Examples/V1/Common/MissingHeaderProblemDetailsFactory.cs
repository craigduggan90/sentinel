using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Exceptions;
using Sentinel.Api.Infrastructure.Errors.Handlers;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Api.Infrastructure.Swagger.Examples.V1.Common;

/// <summary>Builds header-specific examples, e.g. for an If-Match or Idempotency-Key example provider.</summary>
[ExcludeFromCodeCoverage]
public static class MissingHeaderProblemDetailsFactory
{
    public static ProblemDetails GetProblemDetails(string headerName) =>
        MissingHeaderExceptionHandler.GetProblemDetails(new MissingHeaderException(headerName));
}