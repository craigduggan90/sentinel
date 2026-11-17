using Beans.Common.Providers;
using Sentinel.Common.Extensions;
using Sentinel.Common.Providers;
using Sentinel.Domain.Entities.Abstract;

namespace Sentinel.Domain.UnitTests.Entities.Abstract;

public static class PageableEntityBaseTests
{
    private static readonly DateTime Created = new(2026, 3, 4, 5, 6, 7, DateTimeKind.Utc);
    private static readonly DateTime Later = new(2026, 3, 5, 9, 10, 11, DateTimeKind.Utc);

    private static ExampleEntity CreateEntity()
    {
        using var _ = new DateTimeOffsetProviderContext(Created);
        return new ExampleEntity();
    }

    public class Constructor
    {
        [Fact]
        public void InitialisesIdentityAndTimestamps()
        {
            using var id = new IdentifierProviderContext("entity-001");
            using var timestamp = new DateTimeOffsetProviderContext(Created);

            var entity = new ExampleEntity();

            Assert.Equal("entity-001", entity.Id);
            Assert.Equal(Created, entity.DateCreated);
            Assert.Equal(Created, entity.DateModified);
            Assert.Null(entity.DateDeleted);
            Assert.False(entity.IsDirty);
        }

        [Fact]
        public void AssignsCursorFromCreationTime()
        {
            using var _ = new DateTimeOffsetProviderContext(Created);
            var earliestCursor = (long)(Created - DateTime.UnixEpoch).TotalMicroseconds;

            var entity = new ExampleEntity();

            // CursorGenerator never repeats a cursor within a process, so it can be bumped past the creation time.
            Assert.True(entity.Cursor >= earliestCursor);
        }

        [Fact]
        public void AssignsIncreasingCursors_ToEntitiesCreatedAtTheSameTime()
        {
            using var _ = new DateTimeOffsetProviderContext(Created);

            var first = new ExampleEntity();
            var second = new ExampleEntity();

            Assert.True(second.Cursor > first.Cursor);
        }
    }

    public class SetDirty
    {
        [Fact]
        public void SetsIsDirtyAndDateModified()
        {
            var entity = CreateEntity();
            using var _ = new DateTimeOffsetProviderContext(Later);

            entity.SetDirty();

            Assert.True(entity.IsDirty);
            Assert.Equal(Later, entity.DateModified);
            Assert.Equal(Created, entity.DateCreated);
        }
    }

    public class Delete
    {
        [Fact]
        public void SetsDateDeletedAndMarksDirty_WhenNotDeleted()
        {
            var entity = CreateEntity();
            using var _ = new DateTimeOffsetProviderContext(Later);

            entity.Delete();

            Assert.Equal(Later, entity.DateDeleted);
            Assert.Equal(DateTimeKind.Utc, entity.DateDeleted!.Value.Kind);
            Assert.Equal(Later, entity.DateModified);
            Assert.True(entity.IsDirty);
        }

        [Fact]
        public void DoesNothing_WhenAlreadyDeleted()
        {
            var entity = CreateEntity();
            using (new DateTimeOffsetProviderContext(Later))
                entity.Delete();

            using var _ = new DateTimeOffsetProviderContext(Later.AddDays(1));
            entity.Delete();

            Assert.Equal(Later, entity.DateDeleted);
            Assert.Equal(Later, entity.DateModified);
        }
    }

    public class ToStringOverride
    {
        [Fact]
        public void ReturnsSerializedRepresentation()
        {
            var entity = CreateEntity();

            Assert.Equal(entity.AsSerializable().Serialize(), entity.ToString());
        }
    }

    private sealed class ExampleEntity : PageableEntityBase
    {
        public override object AsSerializable() => new { Id, Name = "example" };
    }
}