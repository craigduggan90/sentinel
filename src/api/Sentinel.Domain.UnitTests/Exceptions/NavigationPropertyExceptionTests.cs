using Sentinel.Domain.Exceptions;

namespace Sentinel.Domain.UnitTests.Exceptions;

public static class NavigationPropertyExceptionTests
{
    public class ForUninitialisedProperty
    {
        [Fact]
        public void SetsMessage_NamingTheProperty()
        {
            var exception = NavigationPropertyException.ForUninitialisedProperty("Owner");

            Assert.Equal("Uninitialised Navigation Property: 'Owner'.", exception.Message);
        }

        [Fact]
        public void SetsPropertyName()
        {
            var exception = NavigationPropertyException.ForUninitialisedProperty("Owner");

            Assert.Equal("Owner", exception.PropertyName);
        }

        [Fact]
        public void HasNoInnerException()
        {
            var exception = NavigationPropertyException.ForUninitialisedProperty("Owner");

            Assert.Null(exception.InnerException);
        }
    }
}