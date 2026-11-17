using Sentinel.Api.Exceptions;
using Sentinel.Api.Infrastructure.Errors.Handlers;
using static Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers.ExceptionHandlerTestHelpers;

namespace Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers;

public static class MissingScopeExceptionHandlerTests
{
    public class TryHandleAsync
    {
        [Fact]
        public Task ReturnsFalse_WhenExceptionNotMissingScopeException() => AssertIgnoresAsync(new MissingScopeExceptionHandler());

        [Fact]
        public async Task WritesForbiddenProblemDetails_WhenMissingScopeException()
        {
            var exception = new MissingScopeException("things:write");

            var (statusCode, content) = await HandleAsync(new MissingScopeExceptionHandler(), exception);

            Assert.Equal(403, statusCode);
            AssertCoreFieldsEqual(MissingScopeExceptionHandler.GetProblemDetails(exception), content);
        }
    }

    public class GetProblemDetails
    {
        [Fact]
        public void MapsExceptionToProblemDetails()
        {
            var problemDetails = MissingScopeExceptionHandler.GetProblemDetails(new MissingScopeException("things:write"));

            Assert.Equal("Forbidden", problemDetails.Title);
            Assert.Equal("Required scope was not present: 'things:write'.", problemDetails.Detail);
            Assert.Equal(403, problemDetails.Status);
            Assert.Equal("https://sentinel.api/errors/missing-scope", problemDetails.Type);
        }
    }
}