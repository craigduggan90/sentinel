using Sentinel.Api.Exceptions;
using Sentinel.Api.Infrastructure.Errors.Handlers;
using static Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers.ExceptionHandlerTestHelpers;

namespace Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers;

public static class MissingHeaderExceptionHandlerTests
{
    public class TryHandleAsync
    {
        [Fact]
        public Task ReturnsFalse_WhenExceptionNotMissingHeaderException() => AssertIgnoresAsync(new MissingHeaderExceptionHandler());

        [Fact]
        public async Task WritesPreconditionRequiredProblemDetails_WhenMissingHeaderException()
        {
            var exception = new MissingHeaderException("Idempotency-Key");

            var (statusCode, content) = await HandleAsync(new MissingHeaderExceptionHandler(), exception);

            Assert.Equal(428, statusCode);
            AssertCoreFieldsEqual(MissingHeaderExceptionHandler.GetProblemDetails(exception), content);
        }
    }

    public class GetProblemDetails
    {
        [Fact]
        public void MapsExceptionToProblemDetails()
        {
            var problemDetails = MissingHeaderExceptionHandler.GetProblemDetails(new MissingHeaderException("If-Match"));

            Assert.Equal("Precondition Required", problemDetails.Title);
            Assert.Equal("'If-Match' header value is required.", problemDetails.Detail);
            Assert.Equal(428, problemDetails.Status);
            Assert.Equal("https://sentinel.api/errors/missing-header", problemDetails.Type);
        }
    }
}