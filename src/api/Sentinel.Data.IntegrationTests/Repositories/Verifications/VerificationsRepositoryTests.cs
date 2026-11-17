using Beans.Patchable;
using Microsoft.EntityFrameworkCore;
using Sentinel.Data.Repositories.Verifications;
using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.IntegrationTests.Repositories.Verifications;

public static class VerificationsRepositoryTests
{
    public class CreateAsync : RepositoryTestBase
    {
        [Fact]
        public async Task PersistsTheVerification_OnceSaved()
        {
            var entity = new Verification(SeedDataFactory.Users.GetIdentifier(2), 21);

            var returned = await new VerificationsRepository(Context).CreateAsync(entity, TestContext.Current.CancellationToken);
            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Same(entity, returned);
            await using var context = CreateContext();
            var stored = await context.Verifications.SingleAsync(v => v.Id == entity.Id, TestContext.Current.CancellationToken);
            Assert.Equal(VerificationStatus.Pending, stored.Status);
            Assert.Null(stored.Timestamp);
        }

        [Fact]
        public async Task RejectsAVerificationForAnUnknownUser()
        {
            await new VerificationsRepository(Context).CreateAsync(new Verification("nobody", 18), TestContext.Current.CancellationToken);

            await Assert.ThrowsAsync<DbUpdateException>(() => Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }
    }

    public class UpdateAsync : RepositoryTestBase
    {
        [Fact]
        public async Task PersistsTheReportedOutcome()
        {
            var entity = await Context.Verifications.SingleAsync(v => v.Status == VerificationStatus.Pending, TestContext.Current.CancellationToken);
            entity.Update(Optional<VerificationStatus>.Of(VerificationStatus.Success));

            var returned = await new VerificationsRepository(Context).UpdateAsync(entity, TestContext.Current.CancellationToken);
            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Same(entity, returned);
            await using var context = CreateContext();
            var stored = await context.Verifications.SingleAsync(v => v.Id == entity.Id, TestContext.Current.CancellationToken);
            Assert.Equal(VerificationStatus.Success, stored.Status);
            Assert.Equal(entity.Timestamp, stored.Timestamp);
        }
    }

    public class DeleteAsync : RepositoryTestBase
    {
        [Fact]
        public async Task RemovesTheVerification_OnceSaved()
        {
            var entity = await Context.Verifications.FirstAsync(TestContext.Current.CancellationToken);

            var returned = await new VerificationsRepository(Context).DeleteAsync(entity, TestContext.Current.CancellationToken);
            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Same(entity, returned);
            await using var context = CreateContext();
            Assert.False(await context.Verifications.IgnoreQueryFilters().AnyAsync(v => v.Id == entity.Id, TestContext.Current.CancellationToken));
        }
    }
}