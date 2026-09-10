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

        // BRD: "معاد للتعديل - مسودة" — editing a Returned committee and saving it
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

    // BRD status table only lists Draft/PendingApproval as Cancel sources — not Returned. لا يمكن الغاء لجنة مصدقة او تم ارجاعها للتعديل  - ضمن قواعد العمل
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
    // اقفال اللجنة بعد انتهاء جميع المقابلات الخاصة بها
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
