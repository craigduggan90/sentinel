using Beans.Pageable;
using Sentinel.Data.Repositories.Users;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.IntegrationTests.Repositories.Users;

public static class ReadOnlyUsersRepositoryTests
{
    public class GetByIdAsync : RepositoryTestBase
    {
        private ReadOnlyUsersRepository CreateSut() => new(Context);

        [Fact]
        public async Task ReturnsNull_WhenRecordDoesNotExist()
        {
            Assert.Null(await CreateSut().GetByIdAsync("does-not-exist", TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ReturnsEntity_WhenRecordExists()
        {
            var actual = await CreateSut().GetByIdAsync(SeedDataFactory.Users.GetIdentifier(3), TestContext.Current.CancellationToken);

            Assert.NotNull(actual);
            Assert.Equal(SeedDataFactory.Users.GetEmail(3), actual.Email);
            Assert.Equal(SeedDataFactory.BaseDate.UtcDateTime.AddDays(3), actual.DateCreated);
        }

        [Fact]
        public async Task ReturnsNull_WhenRecordIsSoftDeleted()
        {
            var user = await Context.Users.FindAsync([SeedDataFactory.Users.GetIdentifier(3)], TestContext.Current.CancellationToken);
            user!.Delete();
            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            await using var context = CreateContext();
            var actual = await new ReadOnlyUsersRepository(context).GetByIdAsync(user.Id, TestContext.Current.CancellationToken);

            Assert.Null(actual);
        }
    }

    public class GetByRemoteIdAsync : RepositoryTestBase
    {
        private ReadOnlyUsersRepository CreateSut() => new(Context);

        [Fact]
        public async Task ReturnsNull_WhenRecordDoesNotExist()
        {
            Assert.Null(await CreateSut().GetByRemoteIdAsync("test|nobody", TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ReturnsUserWithScopes_WhenRecordExists()
        {
            var actual = await CreateSut().GetByRemoteIdAsync(SeedDataFactory.Users.GetRemoteId(5), TestContext.Current.CancellationToken);

            Assert.NotNull(actual);
            Assert.Equal(SeedDataFactory.Users.GetIdentifier(5), actual.Id);
            Assert.Equal(UserType.Admin, actual.Type);
            Assert.Equal(["users.manage", "users.read"], actual.Scopes.Select(scope => scope.Scope).Order());
        }

        [Fact]
        public async Task ReturnsUserWithNoScopes_WhenUserHasNone()
        {
            var actual = await CreateSut().GetByRemoteIdAsync(SeedDataFactory.Users.GetRemoteId(1), TestContext.Current.CancellationToken);

            Assert.NotNull(actual);
            Assert.Empty(actual.Scopes);
        }
    }

    public class GetByEmailAsync : RepositoryTestBase
    {
        private ReadOnlyUsersRepository CreateSut() => new(Context);

        [Fact]
        public async Task ReturnsNull_WhenRecordDoesNotExist()
        {
            Assert.Null(await CreateSut().GetByEmailAsync("nobody@test.io", TestContext.Current.CancellationToken));
        }

        [Theory]
        [InlineData("u-00000002@test.io")]
        [InlineData("U-00000002@TEST.IO")]
        public async Task ReturnsEntity_IgnoringCase(string email)
        {
            var actual = await CreateSut().GetByEmailAsync(email, TestContext.Current.CancellationToken);

            Assert.Equal(SeedDataFactory.Users.GetIdentifier(2), actual?.Id);
        }
    }

    public class GetAsync : RepositoryTestBase
    {
        private ReadOnlyUsersRepository CreateSut() => new(Context);

        [Theory]
        [InlineData(UserType.User, 16)]
        [InlineData(UserType.Admin, 4)]
        public async Task ReturnsOnlyTheRequestedType(UserType type, int expectedCount)
        {
            var actual = await CreateSut().GetAsync(type, pageSize: 100, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(expectedCount, actual.Count);
            Assert.All(actual, user => Assert.Equal(type, user.Type));
        }

        [Fact]
        public async Task FiltersByEmail()
        {
            var actual = await CreateSut().GetAsync(UserType.User, email: "U-00000011", cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(SeedDataFactory.Users.GetIdentifier(11), Assert.Single(actual).Id);
        }

        [Fact]
        public async Task FiltersByDate()
        {
            var dateFilter = new DateFilter(
                CreatedFrom: SeedDataFactory.BaseDate.UtcDateTime.AddDays(3),
                CreatedTo: SeedDataFactory.BaseDate.UtcDateTime.AddDays(7));

            var actual = await CreateSut().GetAsync(UserType.User, dateFilter: dateFilter, cancellationToken: TestContext.Current.CancellationToken);

            // Users 3, 4 and 6 - user 5 is an admin, and CreatedTo is exclusive.
            Assert.Equal(
                new[] { 3, 4, 6 }.Select(SeedDataFactory.Users.GetIdentifier),
                actual.Select(user => user.Id));
        }

        [Fact]
        public async Task PagesByCursor_UsingTheDefaultPageSize()
        {
            var sut = CreateSut();

            var first = await sut.GetAsync(UserType.User, pageSize: 10, cancellationToken: TestContext.Current.CancellationToken);
            var second = await sut.GetAsync(UserType.User, cursor: first.Last().Cursor, cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(10, first.Count);
            Assert.Equal(6, second.Count);
            Assert.Empty(first.Select(user => user.Id).Intersect(second.Select(user => user.Id)));
            Assert.True(second.First().Cursor > first.Last().Cursor);
        }
    }
}