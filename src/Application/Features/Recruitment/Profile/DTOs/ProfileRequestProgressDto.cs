using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public record ProfileRequestProgressDto
{
    public int PendingCount { get; init; }
    public int NeedsCorrectionCount { get; init; }
    public int RejectedCount { get; init; }
    public int ApprovedCount { get; init; }
    public DateTimeOffset? LastSubmittedAtUtc { get; init; }
    public DateTimeOffset? LastDecisionAtUtc { get; init; }
    public string? LatestReviewerNote { get; init; }
    public ReviewStatus? LatestStatus { get; init; }
    public bool RequiresUserAction { get; init; }
}