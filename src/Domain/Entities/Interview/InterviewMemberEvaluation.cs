using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewMemberEvaluation), Schema = Schemas.Interview)]
public class InterviewMemberEvaluation : EventEntity
{
    public Guid InterviewAppointmentId { get; set; }
    public InterviewAppointment? InterviewAppointment { get; set; }

    public Guid InterviewCommitteeMemberId { get; set; }
    public InterviewCommitteeMember? InterviewCommitteeMember { get; set; }

    public MemberEvaluationStatus Status { get; set; } = MemberEvaluationStatus.Draft;

    [Column(TypeName = "decimal(6,2)")]
    public decimal? TotalScore { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public Guid? ReopenedById { get; set; }
    public User? ReopenedBy { get; set; }
    public DateTime? ReopenedAt { get; set; }
    public string? ReopenReason { get; set; }

    public string? GeneralNotes { get; set; }

    public ICollection<InterviewMemberEvaluationCriterion> CriterionScores { get; set; } = [];
}

public enum MemberEvaluationStatus
{
    Draft = 1,
    Submitted = 2,
    Reopened = 3
}
