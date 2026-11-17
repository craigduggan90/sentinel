using Microsoft.EntityFrameworkCore;
using Sentinel.Data.Context;
using Sentinel.Data.IntegrationTests.Repositories;
using Sentinel.Data.Services;
using Sentinel.Domain.Entities;

namespace Sentinel.Data.IntegrationTests.Services;

public static class UnitOfWorkTests
{
    private sealed class FixedContextFactory(Func<ApiDbContext> create) : IApiDbContextFactory
    {
        public ApiDbContext CreateDbContext() => create();

        public ApiDbContext CreateDbContext(ContextType contextType) => create();
    }

    public class SaveChangesAsync : RepositoryTestBase
    {
        [Fact]
        public async Task CommitsChangesAcrossRepositoriesTogether()
        {
            await using var sut = new UnitOfWork(new FixedContextFactory(CreateContext));
            var user = new User("test|uow", "uow@test.io");

            await sut.Users.CreateAsync(user, TestContext.Current.CancellationToken);
            await sut.Verifications.CreateAsync(new Verification(user.Id, 18), TestContext.Current.CancellationToken);
            await sut.AuditRecords.CreateAsync(new AuditRecord(user.Id, "created"), TestContext.Current.CancellationToken);
            await sut.SaveChangesAsync(TestContext.Current.CancellationToken);

            await using var context = CreateContext();
            Assert.True(await context.Users.AnyAsync(u => u.Id == user.Id, TestContext.Current.CancellationToken));
            Assert.True(await context.Verifications.AnyAsync(v => v.UserId == user.Id, TestContext.Current.CancellationToken));
            Assert.True(await context.AuditRecords.AnyAsync(a => a.UserId == user.Id, TestContext.Current.CancellationToken));
        }
    }
}