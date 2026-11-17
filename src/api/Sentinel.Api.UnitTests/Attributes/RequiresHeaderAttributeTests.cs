using Microsoft.AspNetCore.Http;
using Sentinel.Api.Attributes;
using Sentinel.Api.Exceptions;

namespace Sentinel.Api.UnitTests.Attributes;

public static class RequiresHeaderAttributeTests
{
    public class OnActionExecuting
    {
        private const string HeaderName = "Idempotency-Key";

        private static Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext CreateContext(
            string? headerName = null, string? headerValue = null)
        {
            var httpContext = new DefaultHttpContext();
            if (headerName is not null && headerValue is not null)
                httpContext.Request.Headers[headerName] = headerValue;

            return ActionContextFactory.CreateExecutingContext(httpContext);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(HeaderName, "")]
        [InlineData(HeaderName, "   ")]
        [InlineData("Some-Other-Header", "some-value")]
        public void ThrowsMissingHeaderException_WhenHeaderAbsentOrBlank(string? headerName, string? headerValue)
        {
            var sut = new RequiresHeaderAttribute(HeaderName);

            var exception = Assert.Throws<MissingHeaderException>(
                () => sut.OnActionExecuting(CreateContext(headerName, headerValue)));

            Assert.Equal(HeaderName, exception.HeaderName);
        }

        [Fact]
        public void DoesNotThrow_WhenHeaderIsPresentAndHasValue()
        {
            var sut = new RequiresHeaderAttribute(HeaderName);

            var exception = Record.Exception(() => sut.OnActionExecuting(CreateContext(HeaderName, "some-value")));

            Assert.Null(exception);
        }
    }
}