using Beans.Common.Providers;
using Sentinel.Common.Extensions;

namespace Sentinel.Domain.Entities;

public class AuditRecord(string userId, string description, object? data = null)
{
    /// <summary>Used by EF Core when materialising records, which sets every property from the stored row.</summary>
    private AuditRecord()
        : this(string.Empty, string.Empty)
    {
    }

    public long Id { get; private set; }

    public string UserId { get; } = userId;

    public DateTime Timestamp { get; } = DateTimeOffsetProvider.UtcNow.UtcDateTime;

    public string Description { get; } = description;

    public string? Data { get; } = data?.Serialize();
}