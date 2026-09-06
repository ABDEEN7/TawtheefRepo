using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewResultReport), Schema = Schemas.Interview)]
public class InterviewResultReport : EventEntity
{
    public Guid InterviewScheduleId { get; set; }
    public InterviewSchedule? InterviewSchedule { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal? AppliedQualificationScore { get; set; }

    public ResultReportStatus Status { get; set; } = ResultReportStatus.Creating;

    public Guid? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? DecisionNotes { get; set; }

    public ICollection<InterviewResultCandidate> Candidates { get; set; } = [];
}

public enum ResultReportStatus
{
    Creating = 1,
    UnderReview = 2,
    Returned = 3,
    Approved = 4,
    Closed = 5
}
