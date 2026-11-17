using Microsoft.EntityFrameworkCore;
using Sentinel.Data.Repositories.AuditRecords;
using Sentinel.Domain.Entities;

namespace Sentinel.Data.IntegrationTests.Repositories.AuditRecords;

public static class AuditRecordsRepositoryTests
{
    public class CreateAsync : RepositoryTestBase
    {
        [Fact]
        public async Task PersistsTheRecord_WithAGeneratedIdAndItsData()
        {
            var entity = new AuditRecord("anyone", "something happened", new { Reason = "test" });

            var returned = await new AuditRecordsRepository(Context).CreateAsync(entity, TestContext.Current.CancellationToken);
            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Same(entity, returned);
            Assert.True(entity.Id > 0);
            await using var context = CreateContext();
            var stored = await context.AuditRecords.SingleAsync(record => record.Id == entity.Id, TestContext.Current.CancellationToken);
            Assert.Equal("""{"reason":"test"}""", stored.Data);
            Assert.Equal(entity.Timestamp, stored.Timestamp);
            Assert.Equal(DateTimeKind.Utc, stored.Timestamp.Kind);
        }

        [Fact]
        public async Task DoesNotRequireTheUserToExist()
        {
            await new AuditRecordsRepository(Context).CreateAsync(new AuditRecord("deleted-long-ago", "kept"), TestContext.Current.CancellationToken);

            var exception = await Record.ExceptionAsync(() => Context.SaveChangesAsync(TestContext.Current.CancellationToken));
            Assert.Null(exception);
        }
    }

    public class GetAsync : RepositoryTestBase
    {
        private ReadOnlyAuditRecordsRepository CreateSut() => new(Context);

        [Fact]
        public async Task ReturnsTheUsersRecords_NewestFirst_UsingTheDefaultPageSize()
        {
            var actual = await CreateSut().GetAsync(SeedDataFactory.Users.GetIdentifier(1), cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(15, actual.Count);
            Assert.All(actual, record => Assert.Equal(SeedDataFactory.Users.GetIdentifier(1), record.UserId));
            Assert.Equal(actual.OrderByDescending(record => record.Id), actual);
            Assert.Equal("event 30", actual.First().Description);
        }

        [Fact]
        public async Task PagesByOffset()
        {
            var actual = await CreateSut().GetAsync(SeedDataFactory.Users.GetIdentifier(1), offset: 2, limit: 3, TestContext.Current.CancellationToken);

            Assert.Equal(["event 26", "event 24", "event 22"], actual.Select(record => record.Description));
        }
    }
}