using Sentinel.Api.Exceptions;

namespace Sentinel.Api.UnitTests.Exceptions;

public static class MissingScopeExceptionTests
{
    public class Constructor
    {
        [Fact]
        public void UsesSingularMessage_WhenOneScopeProvided()
        {
            var exception = new MissingScopeException("things:write");

            Assert.Equal("Required scope was not present: 'things:write'.", exception.Message);
            Assert.Equal(["things:write"], exception.Scopes);
        }

        [Fact]
        public void UsesPluralMessage_WhenMultipleScopesProvided()
        {
            var exception = new MissingScopeException("things:write", "things:read");

            Assert.Equal("Required scopes were not present: 'things:write', 'things:read'.", exception.Message);
            Assert.Equal(["things:write", "things:read"], exception.Scopes);
        }
    }
}