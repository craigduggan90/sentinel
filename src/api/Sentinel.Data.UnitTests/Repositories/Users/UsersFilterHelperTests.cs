using Sentinel.Data.Repositories.Users;
using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.UnitTests.Repositories.Users;

public static class UsersFilterHelperTests
{
    private static IQueryable<User> SeedData =>
        new[]
        {
            new User("test|1", "alice@example.com"),
            new User("test|2", "BOB@example.com"),
            new User("test|3", "admin@example.com", UserType.Admin)
        }.AsQueryable();

    public class ApplyTypeFilter
    {
        [Theory]
        [InlineData(UserType.User, 2)]
        [InlineData(UserType.Admin, 1)]
        public void ReturnsOnlyUsersOfTheGivenType(UserType type, int expectedCount)
        {
            var actual = SeedData.ApplyTypeFilter(type).ToList();

            Assert.Equal(expectedCount, actual.Count);
            Assert.All(actual, user => Assert.Equal(type, user.Type));
        }
    }

    public class ApplyEmailFilter
    {
        [Fact]
        public void ReturnsUnfilteredCollection_WhenValueIsNull()
        {
            var collection = SeedData;

            Assert.Same(collection, collection.ApplyEmailFilter(null));
        }

        [Fact]
        public void ReturnsPartialMatchesIgnoringCase_WhenValueProvided()
        {
            var actual = SeedData.ApplyEmailFilter("bob@").ToList();

            Assert.Equal("BOB@example.com", Assert.Single(actual).Email);
        }
    }
}