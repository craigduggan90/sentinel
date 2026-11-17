using Beans.Common.Providers;
using Beans.Patchable;
using Sentinel.Domain.Entities.Abstract;
using Sentinel.Domain.Enums;
using Sentinel.Domain.Exceptions;

namespace Sentinel.Domain.Entities;

/// <summary>Represents a user verification request.</summary>
/// <param name="userId">The unique identifier of the user.</param>
/// <param name="age">The challenge age of the request.</param>
public class Verification(string userId, int age) : PageableEntityBase
{
    /// <summary>The unique identifier of the user for whom the verification record pertains.</summary>
    public string UserId { get; } = userId;

    /// <summary>The challenge age of the verification request.</summary>
    public int Age { get; } = age;

    /// <summary>The status of the verification.</summary>
    public VerificationStatus Status { get; private set; } = VerificationStatus.Pending;

    /// <summary>The timestamp (UTC) at which a terminal status was set for this verification.</summary>
    public DateTime? Timestamp { get; private set; }

    /// <summary>The user to whom the verification record pertains. </summary>
    /// <exception cref="NavigationPropertyException">when the user is not initialised.</exception>
    public User User
    {
        get => field ?? throw NavigationPropertyException.ForUninitialisedProperty(nameof(User));
        init;
    }

    /// <summary>Update the verification record.</summary>
    /// <param name="status"></param>
    public void Update(Optional<VerificationStatus> status)
    {
        if (Status is VerificationStatus.Success or VerificationStatus.Failure)
            return;

        if (UpdateProperty(nameof(Status), status))
            Timestamp ??= DateTimeOffsetProvider.UtcNow.UtcDateTime;
    }

    /// <inheritdoc />
    public override object AsSerializable() =>
        new { Id, DateCreated, DateModified, DateDeleted };
}