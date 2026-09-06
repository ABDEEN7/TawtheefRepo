using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewCommitteeMember), Schema = Schemas.Interview)]
public class InterviewCommitteeMember : EventEntity
{
    public Guid InterviewCommitteeId { get; set; }
    public InterviewCommittee? InterviewCommittee { get; set; }

    public Guid MemberUserId { get; set; }
    public User? MemberUser { get; set; }

    public CommitteeRole Role { get; set; }
    public bool ParticipatesInEvaluation { get; set; }
    public EvaluationScope EvaluationScope { get; set; } = EvaluationScope.AllAxes;

    public bool CanViewCandidates { get; set; }
    public bool CanAddNotes { get; set; }
    public bool CanSubmitEvaluation { get; set; }
    public bool CanViewOtherEvaluations { get; set; }
    public bool CanViewCommitteeSummary { get; set; }

    // Deactivated, never deleted, so historical evaluations keep resolving to a real member.
    public bool IsActive { get; set; } = true;
    public DateTime? RemovedAt { get; set; }
    public string? RemovalReason { get; set; }

    public ICollection<InterviewCommitteeMemberEvaluationAxis> EvaluationAxes { get; set; } = [];
}

public enum CommitteeRole
{
    Chair = 1,
    Evaluator = 2,
    Observer = 3,
    Admin = 4
}

public enum EvaluationScope
{
    AllAxes = 1,
    SelectedAxes = 2
}
