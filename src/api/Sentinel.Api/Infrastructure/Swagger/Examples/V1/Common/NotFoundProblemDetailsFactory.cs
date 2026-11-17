using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Infrastructure.Errors.Handlers;
using Sentinel.Core.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Api.Infrastructure.Swagger.Examples.V1.Common;

/// <summary>Builds resource-specific examples, e.g. for a per-entity "not found" example provider.</summary>
[ExcludeFromCodeCoverage]
public static class NotFoundProblemDetailsFactory
{
    public static ProblemDetails GetProblemDetails(string resource, string identifier) =>
        NotFoundExceptionHandler.GetProblemDetails(new NotFoundException(resource, identifier));
}