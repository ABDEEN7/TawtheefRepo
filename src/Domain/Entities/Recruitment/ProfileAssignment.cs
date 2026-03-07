using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(ProfileAssignment), Schema = Schemas.Hr)]
[Index(nameof(UserProfileId))]
[Index(nameof(EmployeeId))]
public class ProfileAssignment : EventEntity
{
    public Guid UserProfileId { get; init; }
    public UserProfile? UserProfile { get; init; }

    public Guid EmployeeId { get; init; }
    public EmployeeUser? Employee { get; init; }

    public bool IsActive { get; private set; } = true;
    public DateTimeOffset AssignedAtUtc { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UnassignedAtUtc { get; private set; }

    public static ProfileAssignment Assign(Guid userProfileId, Guid employeeId)
    {
        return new ProfileAssignment
        {
            UserProfileId = userProfileId,
            EmployeeId = employeeId,
            IsActive = true,
            AssignedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        UnassignedAtUtc = DateTimeOffset.UtcNow;
    }
}
