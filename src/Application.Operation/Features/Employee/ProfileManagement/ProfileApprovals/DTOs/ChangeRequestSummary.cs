using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;

public sealed class ChangeRequestSummary
{
    public Guid UserProfileId { get; init; }
    public int Outstanding { get; init; }
    public int Pending { get; init; }
    public int Flagged { get; init; }
    public int Rejected { get; init; }
    public ReviewStatus OverallStatus { get; init; }
    public DateTimeOffset LastUpdatedAtUtc { get; init; }
}
