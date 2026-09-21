using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
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

    // The caller (command handler) must verify the committee is member-editable, the user exists,
    // and the Chair/duplicate-membership uniqueness rules before calling this - all cross-aggregate
    // or sibling-row lookups the entity cannot perform on its own.
    public static InterviewCommitteeMember Create(
        Guid interviewCommitteeId, Guid memberUserId, CommitteeRole role, bool participatesInEvaluation,
        EvaluationScope evaluationScope, bool canViewCandidates, bool canAddNotes, bool canSubmitEvaluation,
        bool canViewOtherEvaluations, bool canViewCommitteeSummary)
    {
        return new InterviewCommitteeMember
        {
            InterviewCommitteeId = interviewCommitteeId,
            MemberUserId = memberUserId,
            Role = role,
            ParticipatesInEvaluation = participatesInEvaluation,
            EvaluationScope = evaluationScope,
            CanViewCandidates = canViewCandidates,
            CanAddNotes = canAddNotes,
            CanSubmitEvaluation = canSubmitEvaluation,
            CanViewOtherEvaluations = canViewOtherEvaluations,
            CanViewCommitteeSummary = canViewCommitteeSummary,
            IsActive = true
        };
    }

    public Result Update(
        CommitteeRole role, bool participatesInEvaluation, EvaluationScope evaluationScope,
        bool canViewCandidates, bool canAddNotes, bool canSubmitEvaluation,
        bool canViewOtherEvaluations, bool canViewCommitteeSummary)
    {
        var editable = InterviewCommittee!.EnsureMembersEditable();
        if (editable.IsFailed)
            return editable;

        Role = role;
        ParticipatesInEvaluation = participatesInEvaluation;
        EvaluationScope = evaluationScope;
        CanViewCandidates = canViewCandidates;
        CanAddNotes = canAddNotes;
        CanSubmitEvaluation = canSubmitEvaluation;
        CanViewOtherEvaluations = canViewOtherEvaluations;
        CanViewCommitteeSummary = canViewCommitteeSummary;

        // No longer scoped to specific axes - stale per-axis assignments would be meaningless data.
        if (evaluationScope != EvaluationScope.SelectedAxes)
            EvaluationAxes.Clear();

        return Result.Ok();
    }

    public Result Remove(string? reason)
    {
        var editable = InterviewCommittee!.EnsureMembersEditable();
        if (editable.IsFailed)
            return editable;

        IsActive = false;
        RemovedAt = DateTime.UtcNow;
        RemovalReason = reason;
        AddDomainEvent(new CommitteeMemberRemovedEvent(InterviewCommittee!, this, DateTimeOffset.Now));
        return Result.Ok();
    }

    // Only meaningful when this member is scoped to SelectedAxes; the axis itself must belong to the
    // committee's template's currently Approved version - a cross-aggregate check the handler performs
    // before calling this.
    public Result<InterviewCommitteeMemberEvaluationAxis> AddEvaluationAxis(Guid interviewTemplateEvaluationAxisId)
    {
        var editable = InterviewCommittee!.EnsureMembersEditable();
        if (editable.IsFailed)
            return Result.Fail<InterviewCommitteeMemberEvaluationAxis>(editable.Errors);

        if (EvaluationScope != EvaluationScope.SelectedAxes)
            return Result.Fail<InterviewCommitteeMemberEvaluationAxis>(new Error(ErrorsCodes.InterviewCommitteeMemberScopeNotSelectedAxes));

        if (EvaluationAxes.Any(a => a.InterviewTemplateEvaluationAxisId == interviewTemplateEvaluationAxisId))
            return Result.Fail<InterviewCommitteeMemberEvaluationAxis>(new Error(ErrorsCodes.InterviewCommitteeMemberEvaluationAxisAlreadyExists));

        var axis = new InterviewCommitteeMemberEvaluationAxis
        {
            InterviewCommitteeMemberId = Id,
            InterviewTemplateEvaluationAxisId = interviewTemplateEvaluationAxisId
        };

        EvaluationAxes.Add(axis);
        return Result.Ok(axis);
    }

    public Result RemoveEvaluationAxis(Guid evaluationAxisId)
    {
        var editable = InterviewCommittee!.EnsureMembersEditable();
        if (editable.IsFailed)
            return editable;

        var axis = EvaluationAxes.FirstOrDefault(a => a.Id == evaluationAxisId);
        if (axis is null)
            return Result.Fail(new Error(ErrorsCodes.InterviewCommitteeMemberEvaluationAxisNotFound));

        EvaluationAxes.Remove(axis);
        return Result.Ok();
    }

    // Replaces this member's whole axis set in one pass - used when the frontend resends the complete
    // per-member axis selection rather than calling Add/RemoveEvaluationAxis one at a time.
    public Result SetEvaluationAxes(IReadOnlyCollection<Guid> interviewTemplateEvaluationAxisIds)
    {
        var editable = InterviewCommittee!.EnsureMembersEditable();
        if (editable.IsFailed)
            return editable;

        // Not scoped to specific axes - any axes here would be meaningless data.
        if (EvaluationScope != EvaluationScope.SelectedAxes)
        {
            EvaluationAxes.Clear();
            return Result.Ok();
        }

        foreach (var axis in EvaluationAxes.Where(a => !interviewTemplateEvaluationAxisIds.Contains(a.InterviewTemplateEvaluationAxisId)).ToList())
            EvaluationAxes.Remove(axis);

        foreach (var axisId in interviewTemplateEvaluationAxisIds)
        {
            if (EvaluationAxes.Any(a => a.InterviewTemplateEvaluationAxisId == axisId))
                continue;

            var addResult = AddEvaluationAxis(axisId);
            if (addResult.IsFailed)
                return Result.Fail(addResult.Errors);
        }

        return Result.Ok();
    }
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
