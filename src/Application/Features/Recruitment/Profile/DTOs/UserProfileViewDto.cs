using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed record UserProfileViewDto
{
    public Guid UserProfileId { get; init; }
    public UserProfileStatus Status { get; init; }

    // Reuse your existing snapshot DTO (it’s fine)
    public ProfileApprovalDataDto Profile { get; init; } = default!;
}