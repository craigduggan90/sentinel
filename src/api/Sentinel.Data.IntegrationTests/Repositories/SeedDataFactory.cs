using Beans.Common.Providers;
using Beans.Patchable;
using Sentinel.Common.Providers;
using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Data.IntegrationTests.Repositories;

public static class SeedDataFactory
{
    public static readonly DateTimeOffset BaseDate = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    public static class Users
    {
        public static string GetIdentifier(int index) => $"u-{index:D8}";

        public static string GetRemoteId(int index) => $"test|{GetIdentifier(index)}";

        public static string GetEmail(int index) => $"{GetIdentifier(index)}@test.io";

        /// <summary>Every fifth user is an admin.</summary>
        public static UserType GetType(int index) => index % 5 == 0 ? UserType.Admin : UserType.User;

        public static User Create(int index)
        {
            using var id = new IdentifierProviderContext(GetIdentifier(index));
            using var created = new DateTimeOffsetProviderContext(BaseDate.AddDays(index));

            return new User(GetRemoteId(index), GetEmail(index), GetType(index));
        }
    }

    public static class Verifications
    {
        public static string GetIdentifier(int userIndex, int index) => $"v-{userIndex:D4}-{index:D4}";

        /// <summary>Creates a verification which is still waiting for its provider to report.</summary>
        public static Verification CreatePending(int userIndex, int index, int age)
        {
            using var id = new IdentifierProviderContext(GetIdentifier(userIndex, index));
            using var created = new DateTimeOffsetProviderContext(BaseDate.AddDays(userIndex).AddHours(index));

            return new Verification(Users.GetIdentifier(userIndex), age);
        }

        /// <summary>Creates a verification which reached <paramref name="status"/> <paramref name="days"/> after creation.</summary>
        public static Verification Create(
            int userIndex,
            int index,
            int age,
            VerificationStatus status,
            int days = 1)
        {
            var created = BaseDate.AddDays(userIndex).AddHours(index);

            Verification verification;
            using (new IdentifierProviderContext(GetIdentifier(userIndex, index)))
            using (new DateTimeOffsetProviderContext(created))
                verification = new Verification(Users.GetIdentifier(userIndex), age);

            using (new DateTimeOffsetProviderContext(created.AddDays(days)))
                verification.Update(Optional<VerificationStatus>.Of(status));

            return verification;
        }
    }
}