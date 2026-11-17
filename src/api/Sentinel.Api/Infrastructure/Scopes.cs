namespace Sentinel.Api.Infrastructure;

public static class Scopes
{
    public const string Authorizer = "cCS0x5zDobQJRyk1IoJ4YZ5leA2vxPu5";

    public static class Users
    {
        public const string Read = "users.read";
        public const string Manage = "users.manage";
        public const string Verify = "users.verify";
    }

    public static class Admins
    {
        public const string Read = "admins.users.read";
        public const string Manage = "admins.users.manage";
        public const string Create = "admins.users.create";
        public const string Delete = "admins.users.delete";
    }
}