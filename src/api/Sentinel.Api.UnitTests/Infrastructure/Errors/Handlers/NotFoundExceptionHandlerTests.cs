using Sentinel.Api.Infrastructure.Errors.Handlers;
using Sentinel.Core.Exceptions;
using static Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers.ExceptionHandlerTestHelpers;

namespace Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers;

public static class NotFoundExceptionHandlerTests
{
    public class TryHandleAsync
    {
        [Fact]
        public Task ReturnsFalse_WhenExceptionNotNotFoundException() => AssertIgnoresAsync(new NotFoundExceptionHandler());

        [Fact]
        public async Task WritesNotFoundProblemDetailsWithExtensions_WhenResourceDetailsProvided()
        {
            var exception = new NotFoundException("Widget", "widget-001");

            var (statusCode, content) = await HandleAsync(new NotFoundExceptionHandler(), exception);

            Assert.Equal(404, statusCode);
            AssertCoreFieldsEqual(NotFoundExceptionHandler.GetProblemDetails(exception), content);
            Assert.Equal("Widget", GetExtensionString(content, "resource"));
            Assert.Equal("widget-001", GetExtensionString(content, "identifier"));
        }

        [Fact]
        public async Task WritesNotFoundProblemDetailsWithoutExtensions_WhenNoResourceDetailsProvided()
        {
            var exception = new NotFoundException();

            var (statusCode, content) = await HandleAsync(new NotFoundExceptionHandler(), exception);

            Assert.Equal(404, statusCode);
            AssertCoreFieldsEqual(NotFoundExceptionHandler.GetProblemDetails(exception), content);
            Assert.DoesNotContain("resource", content.Extensions.Keys);
            Assert.DoesNotContain("identifier", content.Extensions.Keys);
        }
    }

    public class GetProblemDetails
    {
        [Fact]
        public void IncludesOnlyTheResourceType_WhenNoIdentifierProvided()
        {
            var problemDetails = NotFoundExceptionHandler.GetProblemDetails(new NotFoundException("Widget"));

            Assert.Equal("Resource Not Found", problemDetails.Title);
            Assert.Equal("The requested resource was not found.", problemDetails.Detail);
            Assert.Equal(404, problemDetails.Status);
            Assert.Equal("https://sentinel.api/errors/not-found", problemDetails.Type);
            Assert.Equal("Widget", problemDetails.Extensions["resource"]);
            Assert.DoesNotContain("identifier", problemDetails.Extensions.Keys);
        }
    }
}