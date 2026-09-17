using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Interview.Committee;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewCommittee), Schema = Schemas.Interview)]
public class InterviewCommittee : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    // commented this cuz now we linked the commitee directly with the interview template instead of the job interview template
    // and the Commitee is linked with published job from Job Entity..

    //public Guid JobInterviewTemplateId { get; set; }
    //public JobInterviewTemplate? JobInterviewTemplate { get; set; }

    public Guid InterviewTemplateId { get; set; }
    public InterviewTemplate? InterviewTemplate { get; set; }

    public required string NameAr { get; set; }
    public string? NameEn { get; set; }


    // No CommitteeType column here - it's derived from Job.JobCategory (Academic/Administrative/Labor)
    // if the BRD later requires a explicit CommitteeType column, we can add it back in and make it optional,

    //public Guid? CommitteeTypeId { get; set; }
    //public InterviewCommitteeType? CommitteeType { get; set; }
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

    // The caller (command handler) must verify the Job is Published and the InterviewTemplateId exists
    // before calling this - both are cross-aggregate lookups the entity cannot perform itself.
    public static InterviewCommittee Create(
        Guid jobId, Guid interviewTemplateId, string nameAr, string? nameEn,
        string? scopeDescription, string? notes)
    {
        return new InterviewCommittee
        {
            JobId = jobId,
            InterviewTemplateId = interviewTemplateId,
            NameAr = nameAr,
            NameEn = nameEn,
            ScopeDescription = scopeDescription,
            Notes = notes,
            Status = CommitteeStatus.Draft,
            IsActive = true
        };
    }

    // Core fields (name/type/scope) lock as soon as the committee leaves Draft/Returned.
    public Result EnsureCoreEditable() =>
        Status is CommitteeStatus.Draft or CommitteeStatus.Returned
            ? Result.Ok()
            : Result.Fail(new Error(ErrorsCodes.InterviewCommitteeCoreNotEditable));

    // Membership stays editable through Approved, per the BRD's "تعديل اللجنة بعد الاعتماد" —
    // unlike InterviewTemplateVersion, approval does not freeze this aggregate.
    // we will add special case for edit after aproval (the user that have role for edit commitee after approve : like role: EditCommiteeAfterApprove)
    public Result EnsureMembersEditable() =>
        Status is CommitteeStatus.Draft or CommitteeStatus.Returned or CommitteeStatus.Approved
            ? Result.Ok()
            : Result.Fail(new Error(ErrorsCodes.InterviewCommitteeMembersNotEditable));

    public Result Update(string nameAr, string? nameEn, string? scopeDescription, string? notes)
    {
        var editable = EnsureCoreEditable();
        if (editable.IsFailed)
            return editable;

        NameAr = nameAr;
        NameEn = nameEn;
        ScopeDescription = scopeDescription;
        Notes = notes;

        // BRD: "معادة للتعديل → مسودة" — editing a Returned committee and saving it
        // is what moves it back to Draft; there is no separate "revert" action.
        if (Status == CommitteeStatus.Returned)
            Status = CommitteeStatus.Draft;

        return Result.Ok();
    }

    public Result Submit()
    {
        if (Status != CommitteeStatus.Draft)
            return Result.Fail(new Error(ErrorsCodes.InterviewCommitteeNotDraft));

        Status = CommitteeStatus.PendingApproval;
        AddDomainEvent(new CommitteeSubmittedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    public Result Approve(Guid approverId, string? decisionNotes)
    {
        if (Status != CommitteeStatus.PendingApproval)
            return Result.Fail(new Error(ErrorsCodes.InterviewCommitteeNotPendingApproval));

        Status = CommitteeStatus.Approved;
        ApprovedById = approverId;
        ApprovedAt = DateTime.UtcNow;
        DecisionNotes = decisionNotes;
        AddDomainEvent(new CommitteeApprovedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    public Result Return(string reason)
    {
        if (Status != CommitteeStatus.PendingApproval)
            return Result.Fail(new Error(ErrorsCodes.InterviewCommitteeNotPendingApproval));

        Status = CommitteeStatus.Returned;
        DecisionNotes = reason;
        AddDomainEvent(new CommitteeReturnedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    // BRD state table only lists Draft/PendingApproval as Cancel sources — not Returned.
    public Result Cancel(string reason)
    {
        if (Status is not (CommitteeStatus.Draft or CommitteeStatus.PendingApproval))
            return Result.Fail(new Error(ErrorsCodes.InterviewCommitteeNotCancellable));

        Status = CommitteeStatus.Cancelled;
        DecisionNotes = reason;
        // Frees the job's "one current committee" slot (UX_InterviewCommittee_ActiveJob) so a new one can be created.
        IsActive = false;
        AddDomainEvent(new CommitteeCancelledEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    public Result Stop(string? reason)
    {
        if (Status != CommitteeStatus.Approved)
            return Result.Fail(new Error(ErrorsCodes.InterviewCommitteeNotApproved));

        Status = CommitteeStatus.Stopped;
        AddDomainEvent(new CommitteeStoppedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    public Result Reactivate()
    {
        if (Status != CommitteeStatus.Stopped)
            return Result.Fail(new Error(ErrorsCodes.InterviewCommitteeNotStopped));

        Status = CommitteeStatus.Approved;
        AddDomainEvent(new CommitteeReactivatedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    // Per the BRD, this transition is system-triggered once every linked interview is
    // done — this method is the mechanism; what calls it is step 6/7's concern.
    public Result Close()
    {
        if (Status != CommitteeStatus.Approved)
            return Result.Fail(new Error(ErrorsCodes.InterviewCommitteeNotApproved));

        Status = CommitteeStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        // Frees the job's "one current committee" slot (UX_InterviewCommittee_ActiveJob) so a new one can be created.
        IsActive = false;
        AddDomainEvent(new CommitteeClosedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    // Needs the sibling Members in memory to enforce "one active Chair" / "a user sits on the
    // committee once" - the same reason AddAxis lives on InterviewTemplateVersion rather than the child.
    // The caller must still verify the user exists - a cross-aggregate lookup this entity cannot do.
    public Result<InterviewCommitteeMember> AddMember(
        Guid memberUserId, CommitteeRole role, bool participatesInEvaluation, EvaluationScope evaluationScope,
        bool canViewCandidates, bool canAddNotes, bool canSubmitEvaluation, bool canViewOtherEvaluations,
        bool canViewCommitteeSummary)
    {
        var editable = EnsureMembersEditable();
        if (editable.IsFailed)
            return Result.Fail<InterviewCommitteeMember>(editable.Errors);

        if (role == CommitteeRole.Chair && Members.Any(m => m.IsActive && m.Role == CommitteeRole.Chair))
            return Result.Fail<InterviewCommitteeMember>(new Error(ErrorsCodes.InterviewCommitteeChairAlreadyExists));

        if (Members.Any(m => m.IsActive && m.MemberUserId == memberUserId))
            return Result.Fail<InterviewCommitteeMember>(new Error(ErrorsCodes.InterviewCommitteeMemberAlreadyActive));

        var member = InterviewCommitteeMember.Create(
            Id, memberUserId, role, participatesInEvaluation, evaluationScope,
            canViewCandidates, canAddNotes, canSubmitEvaluation, canViewOtherEvaluations, canViewCommitteeSummary);

        member.InterviewCommittee = this;

        Members.Add(member);
        AddDomainEvent(new CommitteeMemberAddedEvent(this, member, DateTimeOffset.Now));
        return Result.Ok(member);
    }

    public Result SetMembers(IReadOnlyCollection<CommitteeMemberInput> members)
    {
        var editable = EnsureMembersEditable();
        if (editable.IsFailed)
            return editable;
        // minimum member , 1 chair and 2 other members (evaluator or observer)
        if (members.Count < CommitteeConstants.MinimumCommitteeMembers)
            return Result.Fail(new Error(ErrorsCodes.InterviewCommitteeMinimumMembersNotMet));

        if (members.Count(m => m.Role == CommitteeRole.Chair) > 1)
            return Result.Fail(new Error(ErrorsCodes.InterviewCommitteeChairAlreadyExists));

        var incomingUserIds = members.Select(m => m.MemberUserId).ToHashSet();
        foreach (var removedMember in Members.Where(m => m.IsActive && !incomingUserIds.Contains(m.MemberUserId)).ToList())
        {
            var removeResult = removedMember.Remove("Removed by committee member list update");
            if (removeResult.IsFailed)
                return removeResult;
        }

        foreach (var input in members)
        {
            var existingMember = Members.FirstOrDefault(m => m.IsActive && m.MemberUserId == input.MemberUserId);

            InterviewCommitteeMember member;
            if (existingMember is not null)
            {
                var updateResult = existingMember.Update(
                    input.Role, input.ParticipatesInEvaluation, input.EvaluationScope,
                    input.CanViewCandidates, input.CanAddNotes, input.CanSubmitEvaluation,
                    input.CanViewOtherEvaluations, input.CanViewCommitteeSummary);
                if (updateResult.IsFailed)
                    return updateResult;

                member = existingMember;
            }
            else
            {
                var addResult = AddMember(
                    input.MemberUserId, input.Role, input.ParticipatesInEvaluation, input.EvaluationScope,
                    input.CanViewCandidates, input.CanAddNotes, input.CanSubmitEvaluation,
                    input.CanViewOtherEvaluations, input.CanViewCommitteeSummary);
                if (addResult.IsFailed)
                    return Result.Fail(addResult.Errors);

                member = addResult.Value;
            }

            var axesResult = member.SetEvaluationAxes(input.EvaluationAxisIds);
            if (axesResult.IsFailed)
                return axesResult;
        }

        return Result.Ok();
    }
}

// A single member row from the frontend's committee table, used to replace the whole roster in one
// call via InterviewCommittee.SetMembers - not a MediatR command, just this aggregate method's input shape.
// EvaluationScope is not carried here: the caller derives it from whether EvaluationAxisIds is empty
// (AllAxes) or not (SelectedAxes) - the same default applies to every role, Chair included, matching
// "by default the Chair evaluates all axes" without needing the frontend to enumerate them.
public sealed record CommitteeMemberInput(
    Guid MemberUserId,
    CommitteeRole Role,
    bool ParticipatesInEvaluation,
    bool CanViewCandidates,
    bool CanAddNotes,
    bool CanSubmitEvaluation,
    bool CanViewOtherEvaluations,
    bool CanViewCommitteeSummary,
    IReadOnlyCollection<Guid> EvaluationAxisIds)
{
    public EvaluationScope EvaluationScope => EvaluationAxisIds.Count > 0 ? EvaluationScope.SelectedAxes : EvaluationScope.AllAxes;
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
