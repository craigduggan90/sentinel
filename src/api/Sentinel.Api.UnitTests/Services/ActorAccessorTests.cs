using Microsoft.AspNetCore.Http;
using Sentinel.Api.Exceptions;
using Sentinel.Api.Services;

namespace Sentinel.Api.UnitTests.Services;

public static class ActorAccessorTests
{
    public class Current
    {
        private static IHttpContextAccessor CreateContextAccessor(HttpContext? httpContext)
        {
            var accessor = Substitute.For<IHttpContextAccessor>();
            accessor.HttpContext.Returns(httpContext);
            return accessor;
        }

        private static HttpContext CreateHttpContext(string? userId = "user-001", string? userName = "display-name")
        {
            var context = new DefaultHttpContext();
            if (userId is not null)
                context.Request.Headers["Sentinel-User-Id"] = userId;
            if (userName is not null)
                context.Request.Headers["Sentinel-User-Name"] = userName;
            return context;
        }

        [Fact]
        public void ThrowsInvalidOperationException_WhenNoHttpContextAvailable()
        {
            var sut = new ActorAccessor(CreateContextAccessor(null));

            Assert.Throws<InvalidOperationException>(() => sut.Current);
        }

        [Theory]
        [InlineData(null, "display-name", "Sentinel-User-Id")]
        [InlineData("   ", "display-name", "Sentinel-User-Id")]
        [InlineData("user-001", null, "Sentinel-User-Name")]
        [InlineData("user-001", "", "Sentinel-User-Name")]
        public void ThrowsMissingHeaderException_WhenHeaderMissingOrBlank(string? userId, string? userName, string expectedHeader)
        {
            var sut = new ActorAccessor(CreateContextAccessor(CreateHttpContext(userId, userName)));

            var exception = Assert.Throws<MissingHeaderException>(() => sut.Current);

            Assert.Equal(expectedHeader, exception.HeaderName);
        }

        [Fact]
        public void ReturnsActor_WithHeaderValues_WhenAllHeadersPresent()
        {
            var sut = new ActorAccessor(CreateContextAccessor(CreateHttpContext()));

            var actor = sut.Current;

            Assert.Equal("user-001", actor.Id);
            Assert.Equal("display-name", actor.DisplayName);
        }

        [Fact]
        public void ReturnsSameInstance_WithoutRereadingHeaders_OnRepeatedAccess()
        {
            var contextAccessor = CreateContextAccessor(CreateHttpContext());
            var sut = new ActorAccessor(contextAccessor);

            var first = sut.Current;
            var second = sut.Current;

            Assert.Same(first, second);
            _ = contextAccessor.Received(1).HttpContext;
        }
    }
}