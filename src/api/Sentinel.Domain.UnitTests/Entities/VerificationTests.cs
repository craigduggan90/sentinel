using Beans.Common.Providers;
using Beans.Patchable;
using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;
using Sentinel.Domain.Exceptions;
using System.Text.Json;

namespace Sentinel.Domain.UnitTests.Entities;

public static class VerificationTests
{
    public abstract class VerificationTestsBase
    {
        protected const string DefaultUserId = "user-001";
        protected const int DefaultAge = 18;

        protected static readonly DateTime Reported = new(2026, 6, 7, 8, 9, 10, DateTimeKind.Utc);

        protected static Verification CreateVerification() => new(DefaultUserId, DefaultAge);

        protected static Verification CreateReportedVerification(VerificationStatus status)
        {
            var verification = CreateVerification();
            using var _ = new DateTimeOffsetProviderContext(Reported);
            verification.Update(Optional<VerificationStatus>.Of(status));
            return verification;
        }
    }

    public class Constructor : VerificationTestsBase
    {
        [Fact]
        public void CreatesPendingVerification_FromParameters()
        {
            var verification = CreateVerification();

            Assert.Equal(DefaultUserId, verification.UserId);
            Assert.Equal(DefaultAge, verification.Age);
            Assert.Equal(VerificationStatus.Pending, verification.Status);
            Assert.Null(verification.Timestamp);
            Assert.False(verification.IsDirty);
        }
    }

    public class UserProperty : VerificationTestsBase
    {
        [Fact]
        public void ThrowsNavigationPropertyException_WhenUserNotSet()
        {
            var verification = CreateVerification();

            var exception = Assert.Throws<NavigationPropertyException>(() => verification.User);

            Assert.Equal(nameof(Verification.User), exception.PropertyName);
        }

        [Fact]
        public void ReturnsAssignedUser_WhenSetViaObjectInitializer()
        {
            var user = new User("auth0|user-001", "user@example.com");

            var verification = new Verification(user.Id, DefaultAge) { User = user };

            Assert.Same(user, verification.User);
        }
    }

    public class Update : VerificationTestsBase
    {
        [Theory]
        [InlineData(VerificationStatus.Success)]
        [InlineData(VerificationStatus.Failure)]
        public void SetsStatusAndTimestamp_WhenPendingVerificationIsReported(VerificationStatus status)
        {
            var verification = CreateReportedVerification(status);

            Assert.Equal(status, verification.Status);
            Assert.Equal(Reported, verification.Timestamp);
            Assert.Equal(DateTimeKind.Utc, verification.Timestamp!.Value.Kind);
            Assert.Equal(Reported, verification.DateModified);
            Assert.True(verification.IsDirty);
        }

        [Fact]
        public void DoesNothing_WhenStatusIsUnset()
        {
            var verification = CreateVerification();

            verification.Update(Optional<VerificationStatus>.Unset);

            Assert.Equal(VerificationStatus.Pending, verification.Status);
            Assert.Null(verification.Timestamp);
            Assert.False(verification.IsDirty);
        }

        [Fact]
        public void DoesNothing_WhenStatusIsUnchanged()
        {
            var verification = CreateVerification();

            verification.Update(Optional<VerificationStatus>.Of(VerificationStatus.Pending));

            Assert.Equal(VerificationStatus.Pending, verification.Status);
            Assert.Null(verification.Timestamp);
            Assert.False(verification.IsDirty);
        }

        [Theory]
        [InlineData(VerificationStatus.Success, VerificationStatus.Failure)]
        [InlineData(VerificationStatus.Success, VerificationStatus.Pending)]
        [InlineData(VerificationStatus.Failure, VerificationStatus.Success)]
        [InlineData(VerificationStatus.Failure, VerificationStatus.Pending)]
        public void DoesNothing_WhenVerificationIsAlreadyTerminal(VerificationStatus terminal, VerificationStatus next)
        {
            var verification = CreateReportedVerification(terminal);
            var dateModified = verification.DateModified;

            using var _ = new DateTimeOffsetProviderContext(Reported.AddDays(1));
            verification.Update(Optional<VerificationStatus>.Of(next));

            Assert.Equal(terminal, verification.Status);
            Assert.Equal(Reported, verification.Timestamp);
            Assert.Equal(dateModified, verification.DateModified);
        }
    }

    public class AsSerializable : VerificationTestsBase
    {
        [Fact]
        public void IncludesIdentityAndTimestampsOnly()
        {
            var verification = CreateReportedVerification(VerificationStatus.Success);

            var serialized = JsonSerializer.SerializeToElement(verification.AsSerializable());

            Assert.Equal(verification.Id, serialized.GetProperty("Id").GetString());
            Assert.Equal(verification.DateCreated, serialized.GetProperty("DateCreated").GetDateTime());
            Assert.Equal(verification.DateModified, serialized.GetProperty("DateModified").GetDateTime());
            Assert.Equal(JsonValueKind.Null, serialized.GetProperty("DateDeleted").ValueKind);
            Assert.False(serialized.TryGetProperty("UserId", out _));
            Assert.False(serialized.TryGetProperty("Age", out _));
            Assert.False(serialized.TryGetProperty("Status", out _));
        }
    }
}