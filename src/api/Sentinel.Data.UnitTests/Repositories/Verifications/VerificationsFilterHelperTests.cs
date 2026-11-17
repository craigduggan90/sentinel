using Beans.Common.Providers;
using Beans.Patchable;
using Sentinel.Data.Repositories.Verifications;
using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.UnitTests.Repositories.Verifications;

public static class VerificationsFilterHelperTests
{
    private static readonly DateTime BaseDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static Verification Create(string userId, int age, VerificationStatus status, int day)
    {
        var verification = new Verification(userId, age);
        using var _ = new DateTimeOffsetProviderContext(BaseDate.AddDays(day));
        verification.Update(Optional<VerificationStatus>.Of(status));
        return verification;
    }

    private static IQueryable<Verification> SeedData =>
        new[]
        {
            new Verification("user-1", 18),
            Create("user-1", 16, VerificationStatus.Success, 1),
            Create("user-1", 21, VerificationStatus.Failure, 2),
            Create("user-2", 18, VerificationStatus.Success, 3)
        }.AsQueryable();

    public class ApplyUserIdFilter
    {
        [Fact]
        public void ReturnsOnlyTheUsersVerifications()
        {
            var actual = SeedData.ApplyUserIdFilter("user-1").ToList();

            Assert.Equal(3, actual.Count);
            Assert.All(actual, verification => Assert.Equal("user-1", verification.UserId));
        }
    }

    public class ApplyStatusFilter
    {
        [Fact]
        public void ReturnsUnfilteredCollection_WhenValueIsNull()
        {
            var collection = SeedData;

            Assert.Same(collection, collection.ApplyStatusFilter(null));
        }

        [Theory]
        [InlineData(VerificationStatus.Pending, 1)]
        [InlineData(VerificationStatus.Success, 2)]
        [InlineData(VerificationStatus.Failure, 1)]
        public void ReturnsOnlyMatchingStatuses_WhenValueProvided(VerificationStatus status, int expectedCount)
        {
            var actual = SeedData.ApplyStatusFilter(status).ToList();

            Assert.Equal(expectedCount, actual.Count);
            Assert.All(actual, verification => Assert.Equal(status, verification.Status));
        }
    }

    public class ApplyMinimumAgeFilter
    {
        [Fact]
        public void ReturnsVerificationsAtOrAboveTheAge()
        {
            var actual = SeedData.ApplyMinimumAgeFilter(18).Select(verification => verification.Age).Order().ToList();

            Assert.Equal([18, 18, 21], actual);
        }
    }

    public class ApplyTimestampFromFilter
    {
        [Fact]
        public void ReturnsVerificationsReachingTheirStatusAtOrAfterTheTime_ExcludingPending()
        {
            var actual = SeedData.ApplyTimestampFromFilter(BaseDate.AddDays(2)).ToList();

            Assert.Equal(2, actual.Count);
            Assert.All(actual, verification => Assert.True(verification.Timestamp >= BaseDate.AddDays(2)));
        }
    }
}