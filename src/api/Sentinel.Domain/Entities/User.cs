using Sentinel.Domain.Entities.Abstract;
using Sentinel.Domain.Enums;

namespace Sentinel.Domain.Entities;

public class User(string remoteId, string email, UserType type = UserType.User)
    : PageableEntityBase
{
    public string RemoteId { get; } = remoteId;

    public string Email { get; } = email;

    public UserType Type { get; } = type;

    public IReadOnlyCollection<UserScope> Scopes { get; init; } = new List<UserScope>();

    public IReadOnlyCollection<Verification> Verifications { get; init; } = new List<Verification>();

    /// <inheritdoc/>
    public override object AsSerializable() => new { Id, DateCreated, DateModified, DateDeleted };
}