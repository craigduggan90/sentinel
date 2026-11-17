using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Infrastructure.Errors.Handlers;
using Sentinel.Common.Extensions;
using Sentinel.Core.Exceptions;
using static Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers.ExceptionHandlerTestHelpers;

namespace Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers;

public static class ValidationExceptionHandlerTests
{
    private static readonly ValidationFailure[] Failures =
    [
        new("one", "one"),
        new("one", "two"),
        new("one", "three"),
        new("two", "one"),
        new("two", "two"),
        new("three", "one")
    ];

    private static readonly Dictionary<string, string[]> ExpectedErrors = new()
    {
        { "one", ["one", "two", "three"] },
        { "two", ["one", "two"] },
        { "three", ["one"] }
    };

    private static Dictionary<string, string[]>? ReadErrors(ProblemDetails problemDetails)
    {
        Assert.True(problemDetails.Extensions.TryGetValue("errors", out var errors));
        return errors.Serialize().Deserialize<Dictionary<string, string[]>>();
    }

    public class TryHandleAsync
    {
        [Fact]
        public Task ReturnsFalse_WhenExceptionNotValidationException() => AssertIgnoresAsync(new ValidationExceptionHandler());

        [Fact]
        public async Task WritesUnprocessableEntity_WhenCommandValidationException()
        {
            var exception = new CommandValidationException(Failures);

            var (statusCode, content) = await HandleAsync(new ValidationExceptionHandler(), exception);

            Assert.Equal(422, statusCode);
            AssertCoreFieldsEqual(ValidationExceptionHandler.GetProblemDetails(422, exception), content);
            Assert.Equivalent(ExpectedErrors, ReadErrors(content), strict: true);
        }

        [Fact]
        public async Task WritesBadRequest_WhenQueryValidationException()
        {
            var exception = new QueryValidationException(Failures);

            var (statusCode, content) = await HandleAsync(new ValidationExceptionHandler(), exception);

            Assert.Equal(400, statusCode);
            AssertCoreFieldsEqual(ValidationExceptionHandler.GetProblemDetails(400, exception), content);
            Assert.Equivalent(ExpectedErrors, ReadErrors(content), strict: true);
        }
    }

    public class GetProblemDetails
    {
        [Fact]
        public void MapsExceptionToProblemDetails()
        {
            var problemDetails = ValidationExceptionHandler.GetProblemDetails(422, new CommandValidationException(Failures));

            Assert.Equal("Validation Error", problemDetails.Title);
            Assert.Equal("One or more validation failures occurred.", problemDetails.Detail);
            Assert.Equal(422, problemDetails.Status);
            Assert.Equal("https://sentinel.api/errors/validation", problemDetails.Type);
        }
    }
}