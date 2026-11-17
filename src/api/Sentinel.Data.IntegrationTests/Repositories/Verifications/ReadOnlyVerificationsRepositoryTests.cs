using Beans.Pageable;
using Sentinel.Data.Repositories.Verifications;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.IntegrationTests.Repositories.Verifications;

public static class ReadOnlyVerificationsRepositoryTests
{
    private static readonly string UserId = SeedDataFactory.Users.GetIdentifier(1);

    public class GetByIdAsync : RepositoryTestBase
    {
        private ReadOnlyVerificationsRepository CreateSut() => new(Context);

        [Fact]
        public async Task ReturnsNull_WhenRecordDoesNotExist()
        {
            Assert.Null(await CreateSut().GetByIdAsync("does-not-exist", TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ReturnsEntity_WhenRecordExists()
        {
            var id = SeedDataFactory.Verifications.GetIdentifier(1, 2);

            var actual = await CreateSut().GetByIdAsync(id, TestContext.Current.CancellationToken);

            Assert.NotNull(actual);
            Assert.Equal(18, actual.Age);
            Assert.Equal(VerificationStatus.Success, actual.Status);
            Assert.Equal(DateTimeKind.Utc, actual.Timestamp!.Value.Kind);
        }
    }

    public class GetLatestAsync : RepositoryTestBase
    {
        private ReadOnlyVerificationsRepository CreateSut() => new(Context);

        private static readonly DateTime LongAgo = SeedDataFactory.BaseDate.UtcDateTime.AddYears(-1);

        [Fact]
        public async Task ReturnsTheMostRecentSuccess_AtOrAboveTheAge()
        {
            var actual = await CreateSut().GetLatestAsync(UserId, 16, VerificationStatus.Success, LongAgo, TestContext.Current.CancellationToken);

            // Both the 16+ and the 18+ successes qualify; the 18+ one was reported later.
            Assert.Equal(SeedDataFactory.Verifications.GetIdentifier(1, 2), actual?.Id);
        }

        [Fact]
        public async Task ReturnsNull_WhenNoSuccessReachesTheAge()
        {
            var actual = await CreateSut().GetLatestAsync(UserId, 21, VerificationStatus.Success, LongAgo, TestContext.Current.CancellationToken);

            Assert.Null(actual);
        }

        [Fact]
        public async Task IgnoresVerificationsReportedBeforeTheWindow()
        {
            var since = SeedDataFactory.BaseDate.UtcDateTime.AddDays(3).AddHours(2);

            var actual = await CreateSut().GetLatestAsync(UserId, 16, VerificationStatus.Success, since, TestContext.Current.CancellationToken);

            Assert.Equal(SeedDataFactory.Verifications.GetIdentifier(1, 2), actual?.Id);

            var tooLate = await CreateSut().GetLatestAsync(UserId, 16, VerificationStatus.Success, since.AddSeconds(1), TestContext.Current.CancellationToken);
            Assert.Null(tooLate);
        }

        [Fact]
        public async Task IgnoresOtherUsersVerifications()
        {
            var actual = await CreateSut().GetLatestAsync("nobody", 16, VerificationStatus.Success, LongAgo, TestContext.Current.CancellationToken);

            Assert.Null(actual);
        }
    }

    public class GetAsync : RepositoryTestBase
    {
        private ReadOnlyVerificationsRepository CreateSut() => new(Context);

        [Fact]
        public async Task ReturnsTheUsersHistory_NewestFirst()
        {
            var actual = await CreateSut().GetAsync(UserId, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(4, actual.Count);
            Assert.Equal(actual.OrderByDescending(verification => verification.Cursor), actual);
        }

        [Fact]
        public async Task FiltersByStatus()
        {
            var actual = await CreateSut().GetAsync(UserId, VerificationStatus.Pending, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(VerificationStatus.Pending, Assert.Single(actual).Status);
        }

        [Fact]
        public async Task FiltersByDate()
        {
            var dateFilter = new DateFilter(ModifiedFrom: SeedDataFactory.BaseDate.UtcDateTime.AddDays(4));

            var actual = await CreateSut().GetAsync(UserId, dateFilter: dateFilter, cancellationToken: TestContext.Current.CancellationToken);

            // Only the 21+ failure was reported (and so modified) that late.
            Assert.Equal(SeedDataFactory.Verifications.GetIdentifier(1, 3), Assert.Single(actual).Id);
        }

        [Fact]
        public async Task PagesByCursor()
        {
            var sut = CreateSut();

            var first = await sut.GetAsync(UserId, pageSize: 3, cancellationToken: TestContext.Current.CancellationToken);
            var second = await sut.GetAsync(UserId, cursor: first.Last().Cursor, pageSize: 3, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(3, first.Count);
            Assert.Single(second);
            Assert.True(second.Single().Cursor < first.Last().Cursor);
        }
    }
}