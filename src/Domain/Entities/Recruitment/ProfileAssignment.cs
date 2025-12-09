using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(ProfileAssignment), Schema = Schemas.Hr)]
public class ProfileAssignment : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }

    public Guid EmployeeId { get; set; }
    public EmployeeUser? Employee { get; set; }

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
