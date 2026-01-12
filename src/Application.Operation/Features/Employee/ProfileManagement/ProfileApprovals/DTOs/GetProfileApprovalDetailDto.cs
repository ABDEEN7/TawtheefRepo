using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;

public class GetProfileApprovalDetailDto
{
    public Guid UserProfileId { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;

    public string? CandidateType { get; set; }
    public string? TargetEntity { get; set; }

    /// <summary>
    /// Snapshot being reviewed now (submitted/latest state)
    /// </summary>
    public ProfileApprovalDataDto Profile { get; set; } = default!;

    public List<ProfileApprovalSectionDto> Sections { get; set; } = [];
}
