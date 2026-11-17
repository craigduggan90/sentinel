using Sentinel.Core.Exceptions;

namespace Sentinel.Core.UnitTests.Exceptions;

public static class NotFoundExceptionTests
{
    private const string ExpectedMessage = "The requested resource was not found.";

    public class Constructor
    {
        [Fact]
        public void SetsMessageOnly_WhenCalledWithNoArguments()
        {
            var exception = new NotFoundException();

            Assert.Equal(ExpectedMessage, exception.Message);
            Assert.Null(exception.ResourceType);
            Assert.Null(exception.ResourceIdentifier);
        }

        [Fact]
        public void SetsResourceTypeFromMemberInfoName_WhenCalledWithMemberInfo()
        {
            var exception = new NotFoundException(typeof(NotFoundException));

            Assert.Equal(ExpectedMessage, exception.Message);
            Assert.Equal(nameof(NotFoundException), exception.ResourceType);
            Assert.Null(exception.ResourceIdentifier);
        }

        [Fact]
        public void SetsResourceType_WhenCalledWithStringType()
        {
            var exception = new NotFoundException("CustomResource");

            Assert.Equal(ExpectedMessage, exception.Message);
            Assert.Equal("CustomResource", exception.ResourceType);
            Assert.Null(exception.ResourceIdentifier);
        }

        [Fact]
        public void SetsResourceTypeAndIdentifier_WhenCalledWithMemberInfoAndIdentifier()
        {
            var exception = new NotFoundException(typeof(NotFoundException), "abc-123");

            Assert.Equal(ExpectedMessage, exception.Message);
            Assert.Equal(nameof(NotFoundException), exception.ResourceType);
            Assert.Equal("abc-123", exception.ResourceIdentifier);
        }

        [Fact]
        public void SetsResourceTypeAndIdentifier_WhenCalledWithStringTypeAndIdentifier()
        {
            var exception = new NotFoundException("CustomResource", "abc-123");

            Assert.Equal(ExpectedMessage, exception.Message);
            Assert.Equal("CustomResource", exception.ResourceType);
            Assert.Equal("abc-123", exception.ResourceIdentifier);
        }

        [Fact]
        public void SetsResourceIdentifierUsingToString_WhenIdentifierIsNotAString()
        {
            var exception = new NotFoundException(typeof(NotFoundException), 42);

            Assert.Equal("42", exception.ResourceIdentifier);
        }
    }
}