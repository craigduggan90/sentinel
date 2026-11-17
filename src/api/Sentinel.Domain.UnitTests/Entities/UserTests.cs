using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;
using System.Text.Json;

namespace Sentinel.Domain.UnitTests.Entities;

public static class UserTests
{
    public abstract class UserTestsBase
    {
        protected const string DefaultRemoteId = "auth0|user-001";
        protected const string DefaultEmail = "user@example.com";

        protected static User CreateUser(UserType? type = null) => type is null
            ? new User(DefaultRemoteId, DefaultEmail)
            : new User(DefaultRemoteId, DefaultEmail, type.Value);
    }

    public class Constructor : UserTestsBase
    {
        [Fact]
        public void CreatesUser_FromParameters()
        {
            var user = CreateUser();

            Assert.Equal(DefaultRemoteId, user.RemoteId);
            Assert.Equal(DefaultEmail, user.Email);
            Assert.Empty(user.Scopes);
            Assert.Empty(user.Verifications);
        }

        [Fact]
        public void DefaultsToUserType_WhenTypeNotProvided()
        {
            Assert.Equal(UserType.User, CreateUser().Type);
        }

        [Fact]
        public void SetsType_WhenProvided()
        {
            Assert.Equal(UserType.Admin, CreateUser(UserType.Admin).Type);
        }
    }

    public class AsSerializable : UserTestsBase
    {
        [Fact]
        public void IncludesIdentityAndTimestamps()
        {
            var user = CreateUser();
            user.Delete();

            var serialized = JsonSerializer.SerializeToElement(user.AsSerializable());

            Assert.Equal(user.Id, serialized.GetProperty("Id").GetString());
            Assert.Equal(user.DateCreated, serialized.GetProperty("DateCreated").GetDateTime());
            Assert.Equal(user.DateModified, serialized.GetProperty("DateModified").GetDateTime());
            Assert.Equal(user.DateDeleted, serialized.GetProperty("DateDeleted").GetDateTime());
        }

        [Fact]
        public void ExcludesPersonalData()
        {
            var serialized = JsonSerializer.SerializeToElement(CreateUser().AsSerializable());

            Assert.False(serialized.TryGetProperty("Email", out _));
            Assert.False(serialized.TryGetProperty("RemoteId", out _));
        }
    }
}