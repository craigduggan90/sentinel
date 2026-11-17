using Beans.Common.Providers;
using Sentinel.Domain.Entities;

namespace Sentinel.Domain.UnitTests.Entities;

public static class AuditRecordTests
{
    public class Constructor
    {
        private static readonly DateTime Recorded = new(2026, 7, 8, 9, 10, 11, DateTimeKind.Utc);

        [Fact]
        public void CreatesAuditRecord_FromParameters()
        {
            var data = new { Reason = "requested" };
            using var _ = new DateTimeOffsetProviderContext(Recorded);

            var record = new AuditRecord("user-001", "User deleted.", data);

            Assert.Equal(0, record.Id);
            Assert.Equal("user-001", record.UserId);
            Assert.Equal("User deleted.", record.Description);
            Assert.Equal("""{"reason":"requested"}""", record.Data);
            Assert.Equal(Recorded, record.Timestamp);
            Assert.Equal(DateTimeKind.Utc, record.Timestamp.Kind);
        }

        [Fact]
        public void LeavesDataNull_WhenNotProvided()
        {
            var record = new AuditRecord("user-001", "User deleted.");

            Assert.Null(record.Data);
        }
    }
}