using Sentinel.Api.Infrastructure.Errors.Handlers;
using Sentinel.Core.Exceptions;
using static Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers.ExceptionHandlerTestHelpers;

namespace Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers;

public static class AccessDeniedExceptionHandlerTests
{
    public class TryHandleAsync
    {
        [Fact]
        public Task ReturnsFalse_WhenExceptionNotAccessDeniedException() => AssertIgnoresAsync(new AccessDeniedExceptionHandler());

        [Fact]
        public async Task WritesForbiddenProblemDetails_WhenAccessDeniedException()
        {
            var exception = new AccessDeniedException("Nope.");

            var (statusCode, content) = await HandleAsync(new AccessDeniedExceptionHandler(), exception);

            Assert.Equal(403, statusCode);
            AssertCoreFieldsEqual(AccessDeniedExceptionHandler.GetProblemDetails(exception), content);
        }
    }

    public class GetProblemDetails
    {
        [Fact]
        public void MapsExceptionToProblemDetails()
        {
            var problemDetails = AccessDeniedExceptionHandler.GetProblemDetails(new AccessDeniedException("Nope."));

            Assert.Equal("Forbidden", problemDetails.Title);
            Assert.Equal("Nope.", problemDetails.Detail);
            Assert.Equal(403, problemDetails.Status);
            Assert.Equal("https://sentinel.api/errors/forbidden", problemDetails.Type);
        }
    }
}