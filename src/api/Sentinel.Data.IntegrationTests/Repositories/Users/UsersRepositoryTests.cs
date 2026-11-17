using Microsoft.EntityFrameworkCore;
using Sentinel.Data.Repositories.Users;
using Sentinel.Domain.Entities;

namespace Sentinel.Data.IntegrationTests.Repositories.Users;

public static class UsersRepositoryTests
{
    public class CreateAsync : RepositoryTestBase
    {
        [Fact]
        public async Task PersistsTheUser_OnceSaved()
        {
            var entity = SeedDataFactory.Users.Create(1000);

            var returned = await new UsersRepository(Context).CreateAsync(entity, TestContext.Current.CancellationToken);
            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Same(entity, returned);
            await using var context = CreateContext();
            Assert.NotNull(await context.Users.FindAsync([entity.Id], TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task RejectsADuplicateEmail_AmongLiveUsers()
        {
            var duplicate = new User("test|someone-else", SeedDataFactory.Users.GetEmail(1));

            await new UsersRepository(Context).CreateAsync(duplicate, TestContext.Current.CancellationToken);

            await Assert.ThrowsAsync<DbUpdateException>(() => Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task AllowsAnEmailToBeReused_OnceItsUserIsSoftDeleted()
        {
            var original = await Context.Users.FindAsync([SeedDataFactory.Users.GetIdentifier(1)], TestContext.Current.CancellationToken);
            original!.Delete();
            await new UsersRepository(Context).UpdateAsync(original, TestContext.Current.CancellationToken);
            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var replacement = new User("test|new-idp", SeedDataFactory.Users.GetEmail(1));
            await new UsersRepository(Context).CreateAsync(replacement, TestContext.Current.CancellationToken);

            var exception = await Record.ExceptionAsync(() => Context.SaveChangesAsync(TestContext.Current.CancellationToken));
            Assert.Null(exception);
        }
    }

    public class UpdateAsync : RepositoryTestBase
    {
        [Fact]
        public async Task MarksTheUserModified()
        {
            var entity = await Context.Users.FirstAsync(TestContext.Current.CancellationToken);
            entity.Delete();

            var returned = await new UsersRepository(Context).UpdateAsync(entity, TestContext.Current.CancellationToken);

            Assert.Same(entity, returned);
            Assert.Equal(EntityState.Modified, Context.Entry(entity).State);
        }
    }

    public class DeleteAsync : RepositoryTestBase
    {
        [Fact]
        public async Task RemovesTheUserAndCascadesToTheirScopesAndVerifications()
        {
            var userId = SeedDataFactory.Users.GetIdentifier(5);
            var entity = await Context.Users.SingleAsync(user => user.Id == userId, TestContext.Current.CancellationToken);

            var returned = await new UsersRepository(Context).DeleteAsync(entity, TestContext.Current.CancellationToken);
            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Same(entity, returned);
            await using var context = CreateContext();
            Assert.False(await context.Users.IgnoreQueryFilters().AnyAsync(user => user.Id == userId, TestContext.Current.CancellationToken));
            Assert.False(await context.UserScopes.IgnoreQueryFilters().AnyAsync(scope => scope.UserId == userId, TestContext.Current.CancellationToken));
            Assert.False(await context.Verifications.IgnoreQueryFilters().AnyAsync(v => v.UserId == userId, TestContext.Current.CancellationToken));
        }
    }
}