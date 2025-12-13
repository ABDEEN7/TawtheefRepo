namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;

public class ProfileApprovalDetailDto
{
    public Guid UserProfileId { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;

    public string? CandidateType { get; set; }
    public string? TargetEntity { get; set; }

    public int? SubmissionVersion { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }

    /// <summary>
    /// Snapshot being reviewed now (submitted/latest state)
    /// </summary>
    public ProfileApprovalDataDto Profile { get; set; } = default!;

    /// <summary>
    /// Last approved snapshot (null in initial review)
    /// </summary>
    public ProfileApprovalDataDto? ApprovedProfile { get; set; }

    public List<ProfileApprovalSectionDto> Sections { get; set; } = [];

    /// <summary>
    /// True when user has no approved profile yet
    /// </summary>
    public bool IsInitialReview { get; set; }

    /// <summary>
    /// True when endpoint is changes-only
    /// </summary>
    public bool IsPartialReview { get; set; }
}
