using Sentinel.Api.Exceptions;

namespace Sentinel.Api.UnitTests.Exceptions;

public static class MissingHeaderExceptionTests
{
    public class Constructor
    {
        [Fact]
        public void SetsHeaderNameAndMessage()
        {
            var exception = new MissingHeaderException("If-Match");

            Assert.Equal("If-Match", exception.HeaderName);
            Assert.Equal("'If-Match' header value is required.", exception.Message);
        }
    }
}