using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;

public sealed class FullReviewSummary
{
    public Guid UserProfileId { get; init; }
    public int PendingSections { get; init; }
    public int FlaggedSections { get; init; }
    public int ApprovedSections { get; init; }
    public ReviewStatus OverallStatus { get; init; }
    public DateTimeOffset LastUpdatedAtUtc { get; init; }
}
