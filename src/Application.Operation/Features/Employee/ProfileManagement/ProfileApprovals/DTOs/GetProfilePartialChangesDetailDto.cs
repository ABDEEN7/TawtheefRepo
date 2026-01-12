using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;

public sealed class GetProfilePartialChangesDetailDto
{
    public Guid UserProfileId { get; init; }
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;

    public string? CandidateType { get; init; }
    public string? TargetEntity { get; init; }

    public ProfileApprovalDataDto Profile { get; init; } = default!;
    public List<ProfileApprovalSectionDto> Sections { get; init; } = [];
}

