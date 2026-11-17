using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Infrastructure.Errors.Handlers;
using Sentinel.Core.Exceptions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Api.Infrastructure.Swagger.Examples.V1.Common;

[ExcludeFromCodeCoverage]
public class CommandValidationProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples() =>
        ValidationExceptionHandler.GetProblemDetails(
            ValidationExceptionHandler.CommandValidationStatusCode,
            new CommandValidationException(
            [
                new ValidationFailure("fieldName", "first validation error"),
                new ValidationFailure("fieldName", "second validation error")
            ]));
}