using Sentinel.Domain.Entities;
using Sentinel.Domain.Exceptions;

namespace Sentinel.Domain.UnitTests.Entities;

public static class UserScopeTests
{
    public class Constructor
    {
        [Fact]
        public void CreatesUserScope_FromParameters()
        {
            var userScope = new UserScope("user-001", "users.read");

            Assert.Equal("user-001", userScope.UserId);
            Assert.Equal("users.read", userScope.Scope);
        }
    }

    public class UserProperty
    {
        [Fact]
        public void ThrowsNavigationPropertyException_WhenUserNotSet()
        {
            var userScope = new UserScope("user-001", "users.read");

            var exception = Assert.Throws<NavigationPropertyException>(() => userScope.User);

            Assert.Equal(nameof(UserScope.User), exception.PropertyName);
        }

        [Fact]
        public void ReturnsAssignedUser_WhenSetViaObjectInitializer()
        {
            var user = new User("auth0|user-001", "user@example.com");

            var userScope = new UserScope(user.Id, "users.read") { User = user };

            Assert.Same(user, userScope.User);
        }
    }
}