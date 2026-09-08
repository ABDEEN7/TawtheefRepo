using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewResultCandidate), Schema = Schemas.Interview)]
public class InterviewResultCandidate : EventEntity
{
    public Guid InterviewResultReportId { get; set; }
    public InterviewResultReport? InterviewResultReport { get; set; }

    public Guid InterviewAppointmentId { get; set; }
    public InterviewAppointment? InterviewAppointment { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal FinalScore { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal? QualificationScore { get; set; }

    public bool IsQualified { get; set; }
    public CalculationMethod CalculationMethod { get; set; }

    public FinalDecision? FinalDecision { get; set; }
    public string? DecisionReason { get; set; }
    public Guid? DecidedById { get; set; }
    public User? DecidedBy { get; set; }
    public DateTime? DecidedAt { get; set; }

    public DateTime SnapshotAt { get; set; }

    public ICollection<InterviewResultCandidateAxis> Axes { get; set; } = [];
}

public enum FinalDecision
{
    CandidateForHiringProcess = 1,
    WaitingList = 2,
    Rejected = 3,
    NeedsAction = 4,
    NoShow = 5
}
