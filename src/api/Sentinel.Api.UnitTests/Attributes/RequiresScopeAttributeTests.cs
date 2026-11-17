using Microsoft.AspNetCore.Http;
using Sentinel.Api.Attributes;
using Sentinel.Api.Exceptions;
using Sentinel.Common;

namespace Sentinel.Api.UnitTests.Attributes;

public static class RequiresScopeAttributeTests
{
    public class OnActionExecuting
    {
        private const string RequiredScope = "things:write";

        private static Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext CreateContext(string? scopeHeaderValue)
        {
            var httpContext = new DefaultHttpContext();
            if (scopeHeaderValue is not null)
                httpContext.Request.Headers[Constants.ScopeHeaderKey] = scopeHeaderValue;

            return ActionContextFactory.CreateExecutingContext(httpContext);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("things:read")]
        [InlineData("things:write-all")]
        public void ThrowsMissingScopeException_WhenScopeNotPresent(string? scopeHeaderValue)
        {
            var sut = new RequiresScopeAttribute(RequiredScope);

            var exception = Assert.Throws<MissingScopeException>(() => sut.OnActionExecuting(CreateContext(scopeHeaderValue)));

            Assert.Equal([RequiredScope], exception.Scopes);
        }

        [Theory]
        [InlineData("things:write")]
        [InlineData("things:read, things:write, things:delete")]
        [InlineData("things:read,things:write")]
        [InlineData("THINGS:WRITE")]
        public void DoesNotThrow_WhenScopePresent(string scopeHeaderValue)
        {
            var sut = new RequiresScopeAttribute(RequiredScope);

            var exception = Record.Exception(() => sut.OnActionExecuting(CreateContext(scopeHeaderValue)));

            Assert.Null(exception);
        }
    }
}