using Beans.Common.Providers;
using Beans.Pageable;
using Beans.Patchable;
using Sentinel.Common.Extensions;
using Sentinel.Common.Providers;
using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Entities.Abstract;

public abstract class PageableEntityBase
    : PatchableEntity, IHasCursor, IHasCreatedTimestamp, IHasModifiedTimestamp, ISerializableEntity
{
    protected PageableEntityBase()
    {
        Id = IdentifierProvider.Generate();
        Cursor = CursorGenerator.Next();
        DateCreated = DateTimeOffsetProvider.UtcNow.UtcDateTime;
        DateModified = DateCreated;
    }

    public string Id { get; }

    public long Cursor { get; }

    public DateTime DateCreated { get; }

    public DateTime DateModified { get; private set; }

    /// <summary>The deletion timestamp of the entity.</summary>
    public DateTime? DateDeleted { get; private set; }

    /// <summary>Delete this entity.</summary>
    public virtual void Delete() => SoftDelete();

    /// <summary>Marks the entity as deleted.</summary>
    protected virtual void SoftDelete()
    {
        if (DateDeleted.HasValue)
            return;

        DateDeleted = DateTimeOffsetProvider.Now.UtcDateTime;
        SetDirty();
    }

    protected virtual void SetDateModified() =>
        DateModified = DateTimeOffsetProvider.UtcNow.UtcDateTime;

    /// <inheritdoc />
    public override void SetDirty()
    {
        base.SetDirty();
        SetDateModified();
    }

    /// <inheritdoc />
    public abstract object AsSerializable();

    /// <inheritdoc />
    public override string ToString() => AsSerializable().Serialize();
}