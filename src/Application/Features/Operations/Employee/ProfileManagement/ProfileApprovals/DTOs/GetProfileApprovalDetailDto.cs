using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;

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

    public List<SectionReviewDto> Sections { get; set; } = [];
}

public class SectionReviewDto
{
    public ProfileSection Section { get; init; }
    public ReviewStatus Status { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset? ReviewedAtUtc { get; set; }
}
