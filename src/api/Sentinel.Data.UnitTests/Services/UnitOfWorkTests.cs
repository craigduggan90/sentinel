using Microsoft.EntityFrameworkCore;
using Sentinel.Data.Context;
using Sentinel.Data.Services;

namespace Sentinel.Data.UnitTests.Services;

public static class UnitOfWorkTests
{
    // A context is constructed but never connected - nothing here touches a database.
    private static ApiDbContext CreateContext()
        => new(new DbContextOptionsBuilder<ApiDbContext>().UseSqlite("Data Source=:memory:").Options);

    private static IApiDbContextFactory CreateFactory(ApiDbContext context)
    {
        var factory = Substitute.For<IApiDbContextFactory>();
        factory.CreateDbContext(ContextType.ReadWrite).Returns(context);
        return factory;
    }

    public class Context
    {
        [Fact]
        public void CreatesReadWriteContext_OnFirstAccess()
        {
            var context = CreateContext();
            var factory = CreateFactory(context);
            var sut = new UnitOfWork(factory);

            Assert.Same(context, sut.Context);
            factory.Received(1).CreateDbContext(ContextType.ReadWrite);
        }

        [Fact]
        public void ReusesContext_OnSubsequentAccess()
        {
            var factory = CreateFactory(CreateContext());
            var sut = new UnitOfWork(factory);

            Assert.Same(sut.Context, sut.Context);
            factory.Received(1).CreateDbContext(Arg.Any<ContextType>());
        }

        [Fact]
        public void DoesNotCreateContext_UntilAccessed()
        {
            var factory = CreateFactory(CreateContext());

            _ = new UnitOfWork(factory);

            factory.DidNotReceiveWithAnyArgs().CreateDbContext(default);
        }
    }

    public class Repositories
    {
        [Fact]
        public void CreatesUsersRepository_OnFirstAccess_AndReusesIt()
        {
            var sut = new UnitOfWork(CreateFactory(CreateContext()));

            Assert.NotNull(sut.Users);
            Assert.Same(sut.Users, sut.Users);
        }

        [Fact]
        public void CreatesVerificationsRepository_OnFirstAccess_AndReusesIt()
        {
            var sut = new UnitOfWork(CreateFactory(CreateContext()));

            Assert.NotNull(sut.Verifications);
            Assert.Same(sut.Verifications, sut.Verifications);
        }

        [Fact]
        public void CreatesAuditRecordsRepository_OnFirstAccess_AndReusesIt()
        {
            var sut = new UnitOfWork(CreateFactory(CreateContext()));

            Assert.NotNull(sut.AuditRecords);
            Assert.Same(sut.AuditRecords, sut.AuditRecords);
        }

        [Fact]
        public void SharesTheUnitOfWorkContext_AcrossRepositories()
        {
            var factory = CreateFactory(CreateContext());
            var sut = new UnitOfWork(factory);

            _ = sut.Users;
            _ = sut.Verifications;
            _ = sut.AuditRecords;

            factory.Received(1).CreateDbContext(ContextType.ReadWrite);
        }
    }

    public class SaveChangesAsync
    {
        [Fact]
        public async Task SavesThroughTheUnitOfWorkContext()
        {
            var context = CreateContext();
            var sut = new UnitOfWork(CreateFactory(context));

            await sut.SaveChangesAsync(TestContext.Current.CancellationToken);

            // With nothing tracked there's nothing to write, but the context was created and used.
            Assert.Same(context, sut.Context);
            Assert.Empty(context.ChangeTracker.Entries());
        }
    }

    public class Dispose
    {
        [Fact]
        public void DisposesContext_IfInitialized()
        {
            var context = CreateContext();
            var sut = new UnitOfWork(CreateFactory(context));
            _ = sut.Context;

            sut.Dispose();

            Assert.Throws<ObjectDisposedException>(() => context.ChangeTracker);
        }

        [Fact]
        public void DoesNothing_IfContextNotInitialized()
        {
            var context = CreateContext();
            var factory = CreateFactory(context);
            var sut = new UnitOfWork(factory);

            sut.Dispose();

            _ = context.ChangeTracker; // must not throw
            factory.DidNotReceiveWithAnyArgs().CreateDbContext(default);
        }
    }

    public class DisposeAsync
    {
        [Fact]
        public async Task DisposesContext_IfInitialized()
        {
            var context = CreateContext();
            var sut = new UnitOfWork(CreateFactory(context));
            _ = sut.Context;

            await sut.DisposeAsync();

            Assert.Throws<ObjectDisposedException>(() => context.ChangeTracker);
        }

        [Fact]
        public async Task DoesNothing_IfContextNotInitialized()
        {
            var context = CreateContext();
            var factory = CreateFactory(context);
            var sut = new UnitOfWork(factory);

            await sut.DisposeAsync();

            _ = context.ChangeTracker; // must not throw
            factory.DidNotReceiveWithAnyArgs().CreateDbContext(default);
        }
    }
}