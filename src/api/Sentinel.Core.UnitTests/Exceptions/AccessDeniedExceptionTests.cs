using Sentinel.Core.Exceptions;

namespace Sentinel.Core.UnitTests.Exceptions;

public static class AccessDeniedExceptionTests
{
    public class Constructor
    {
        [Fact]
        public void UsesDefaultMessage_WhenNoArgumentsProvided()
        {
            var exception = new AccessDeniedException();

            Assert.Equal("Access to the requested resource or action is denied.", exception.Message);
            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void UsesSuppliedMessage_WhenProvided()
        {
            var exception = new AccessDeniedException("Only the subject may do that.");

            Assert.Equal("Only the subject may do that.", exception.Message);
            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void SetsInnerException_WhenProvided()
        {
            var inner = new InvalidOperationException("inner");

            var exception = new AccessDeniedException("message", inner);

            Assert.Same(inner, exception.InnerException);
        }
    }
}