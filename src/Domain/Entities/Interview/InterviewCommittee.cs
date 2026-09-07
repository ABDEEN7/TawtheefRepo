using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewCommittee), Schema = Schemas.Interview)]
public class InterviewCommittee : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid JobInterviewTemplateId { get; set; }
    public JobInterviewTemplate? JobInterviewTemplate { get; set; }

    public required string NameAr { get; set; }
    public string? NameEn { get; set; }

    public Guid? CommitteeTypeId { get; set; }
    public InterviewCommitteeType? CommitteeType { get; set; }
    public string? ScopeDescription { get; set; }
    public string? Notes { get; set; }

    public CommitteeStatus Status { get; set; } = CommitteeStatus.Draft;

    // JobId is not UNIQUE alone; it's JobId + IsActive with a filter on Active. This allows for history while ensuring there is only one Current Committee per Job.
    public bool IsActive { get; set; } = true;

    public Guid? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? DecisionNotes { get; set; }

    public ICollection<InterviewCommitteeMember> Members { get; set; } = [];
}

public enum CommitteeStatus
{
    Draft = 1,
    PendingApproval = 2,
    Returned = 3,
    Approved = 4,
    Stopped = 5,
    Closed = 6,
    Cancelled = 7
}
