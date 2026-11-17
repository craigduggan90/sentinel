using Sentinel.Data.IntegrationTests.TestHelpers;
using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.IntegrationTests.Repositories;

public abstract class RepositoryTestBase : DatabaseAwareTestBase
{
    public const int UserCount = 20;

    protected IReadOnlyCollection<User> Users { get; private set; } = [];

    protected User GetUser(int index) => Users.Single(user => user.Id == SeedDataFactory.Users.GetIdentifier(index));

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();
        var cancellationToken = TestContext.Current.CancellationToken;

        Users = Enumerable.Range(1, UserCount).Select(SeedDataFactory.Users.Create).ToArray();
        await Context.Users.AddRangeAsync(Users, cancellationToken);

        // Admins hold every scope; users hold none.
        await Context.UserScopes.AddRangeAsync(
            Users.Where(user => user.Type == UserType.Admin)
                .SelectMany(user => new[] { "users.read", "users.manage" }.Select(scope => new UserScope(user.Id, scope))),
            cancellationToken);

        // User 1 has a full history: a pending check, a failure, and successes at two ages, a day apart.
        await Context.Verifications.AddRangeAsync(
        [
            SeedDataFactory.Verifications.Create(1, 1, 16, VerificationStatus.Success, days: 1),
            SeedDataFactory.Verifications.Create(1, 2, 18, VerificationStatus.Success, days: 2),
            SeedDataFactory.Verifications.Create(1, 3, 21, VerificationStatus.Failure, days: 3),
            SeedDataFactory.Verifications.CreatePending(1, 4, 21)
        ], cancellationToken);

        // Every other user has one successful 18+ verification.
        await Context.Verifications.AddRangeAsync(
            Enumerable.Range(2, UserCount - 1)
                .Select(index => SeedDataFactory.Verifications.Create(index, 1, 18, VerificationStatus.Success)),
            cancellationToken);

        await Context.AuditRecords.AddRangeAsync(
            Enumerable.Range(1, 30).Select(index => new AuditRecord(
                SeedDataFactory.Users.GetIdentifier(index % 2 == 0 ? 1 : 2),
                $"event {index:D2}",
                new { Index = index })),
            cancellationToken);

        await Context.SaveChangesAsync(cancellationToken);
        Context.ChangeTracker.Clear();
    }
}