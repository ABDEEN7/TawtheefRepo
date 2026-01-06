using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public record ProfileOverviewDto
{
    public Guid UserProfileId { get; init; }
    public UserProfileStatus Status { get; init; }
    public ProfileApprovalDataDto? ApprovedProfile { get; init; }
    public ProfileRequestProgressDto RequestProgress { get; init; } = new();
    public IReadOnlyList<ProfileOverviewSectionDto> Sections { get; init; } = [];
    public bool HasPendingChanges => Sections.Any(s => s.HasPendingChanges);
}
