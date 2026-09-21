using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Interview.Schedule;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewSchedule), Schema = Schemas.Interview)]
public class InterviewSchedule : EventEntity
{
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public Guid InterviewTemplateId { get; set; }
    public InterviewTemplate? InterviewTemplate { get; set; }

    public required string TitleAr { get; set; }
    public string? TitleEn { get; set; }

    public InterviewType DefaultInterviewType { get; set; }
    public int DefaultDurationMinutes { get; set; }
    public int DefaultBufferMinutes { get; set; }

    public ScheduleStatus Status { get; set; } = ScheduleStatus.Draft;

    public Guid? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? DecisionNotes { get; set; }

    public ICollection<InterviewAppointment> Appointments { get; set; } = [];

    public static InterviewSchedule Create(
        Guid jobId, Guid interviewTemplateId, string titleAr, string? titleEn,
        InterviewType defaultInterviewType, int defaultDurationMinutes, int defaultBufferMinutes)
    {
        return new InterviewSchedule
        {
            JobId = jobId,
            InterviewTemplateId = interviewTemplateId,
            TitleAr = titleAr,
            TitleEn = titleEn,
            DefaultInterviewType = defaultInterviewType,
            DefaultDurationMinutes = defaultDurationMinutes,
            DefaultBufferMinutes = defaultBufferMinutes,
            Status = ScheduleStatus.Draft
        };
    }

    // Draft, Proposed and Returned are editable; editing a Proposed or Returned schedule
    // bounces it back to Draft (BRD: "مقترح → مسودة: تعديل بيانات الجدول").
    public Result EnsureCoreEditable() =>
        Status is ScheduleStatus.Draft or ScheduleStatus.Proposed or ScheduleStatus.Returned
            ? Result.Ok()
            : Result.Fail(new Error(ErrorsCodes.InterviewScheduleNotEditable));

    public Result Update(string titleAr, string? titleEn, InterviewType defaultInterviewType, int defaultDurationMinutes, int defaultBufferMinutes)
    {
        var editable = EnsureCoreEditable();
        if (editable.IsFailed)
            return editable;

        TitleAr = titleAr;
        TitleEn = titleEn;
        DefaultInterviewType = defaultInterviewType;
        DefaultDurationMinutes = defaultDurationMinutes;
        DefaultBufferMinutes = defaultBufferMinutes;

        // Editing regenerates the appointment list from scratch - the handler rebuilds
        // Appointments before/after calling this; re-entering Draft is what allows that.
        if (Status is ScheduleStatus.Proposed or ScheduleStatus.Returned)
            Status = ScheduleStatus.Draft;

        return Result.Ok();
    }

    // Called after slots are generated and appointments are attached. System-triggered
    // per the BRD; no approval-action row, no domain event - logged to InterviewAuditLog
    // by the handler instead.
    public Result Propose()
    {
        if (Status != ScheduleStatus.Draft)
            return Result.Fail(new Error(ErrorsCodes.InterviewScheduleNotDraft));

        Status = ScheduleStatus.Proposed;
        return Result.Ok();
    }

    public Result Submit()
    {
        if (Status != ScheduleStatus.Proposed)
            return Result.Fail(new Error(ErrorsCodes.InterviewScheduleNotProposed));

        Status = ScheduleStatus.PendingApproval;
        AddDomainEvent(new ScheduleSubmittedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    public Result Approve(Guid approverId, string? decisionNotes)
    {
        if (Status != ScheduleStatus.PendingApproval)
            return Result.Fail(new Error(ErrorsCodes.InterviewScheduleNotPendingApproval));

        Status = ScheduleStatus.Approved;
        ApprovedById = approverId;
        ApprovedAt = DateTime.UtcNow;
        DecisionNotes = decisionNotes;
        AddDomainEvent(new ScheduleApprovedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    public Result Return(string reason)
    {
        if (Status != ScheduleStatus.PendingApproval)
            return Result.Fail(new Error(ErrorsCodes.InterviewScheduleNotPendingApproval));

        Status = ScheduleStatus.Returned;
        DecisionNotes = reason;
        AddDomainEvent(new ScheduleReturnedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    // BRD state table only lists Draft/Proposed/Approved as Cancel sources.
    public Result Cancel(string reason)
    {
        if (Status is not (ScheduleStatus.Draft or ScheduleStatus.Proposed or ScheduleStatus.Approved))
            return Result.Fail(new Error(ErrorsCodes.InterviewScheduleNotCancellable));

        Status = ScheduleStatus.Cancelled;
        DecisionNotes = reason;
        AddDomainEvent(new ScheduleCancelledEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    // System-triggered "when interview time arrives" per the BRD. Exposed as its own
    // command for manual/administrative triggering.
    // we can set auto job to set the status to ready for execution when the interview time arrives.
    public Result MarkReadyForExecution()
    {
        if (Status != ScheduleStatus.Approved)
            return Result.Fail(new Error(ErrorsCodes.InterviewScheduleNotApproved));

        Status = ScheduleStatus.ReadyForExecution;
        return Result.Ok();
    }

    public Result StartExecution()
    {
        if (Status != ScheduleStatus.ReadyForExecution)
            return Result.Fail(new Error(ErrorsCodes.InterviewScheduleNotReadyForExecution));

        Status = ScheduleStatus.InProgress;
        return Result.Ok();
    }

    public Result Close()
    {
        if (Status != ScheduleStatus.InProgress)
            return Result.Fail(new Error(ErrorsCodes.InterviewScheduleNotInProgress));

        Status = ScheduleStatus.Closed;
        return Result.Ok();
    }

    // Builds and attaches one child appointment for a slot the Application-layer planner
    // (ScheduleAppointmentPlanner) already validated for conflicts/eligibility - this entity
    // trusts that work is done; it just builds the row and wires the back-reference immediately
    // (EF only fixes that up post-SaveChanges otherwise - see the Committee/Member lesson).
    // invitationId null means an excess-capacity slot that stays open ("Held") for later use.
    public InterviewAppointment AddAppointment(
        Guid interviewCommitteeId, InterviewType interviewType, Guid? roomId,
        string? remoteMeetingUrl, string? remoteMeetingInstructions,
        DateTime startAt, DateTime endAt, Guid? invitationId, Guid? rescheduledFromAppointmentId = null)
    {
        var appointment = InterviewAppointment.Create(
            Id, interviewCommitteeId, interviewType, roomId, remoteMeetingUrl, remoteMeetingInstructions, startAt, endAt,
            rescheduledFromAppointmentId);
        appointment.InterviewSchedule = this;
        Appointments.Add(appointment);

        if (invitationId is not null)
            appointment.AssignCandidate(invitationId.Value);

        return appointment;
    }
}

public enum ScheduleStatus
{
    Draft = 1,
    Proposed = 2,
    PendingApproval = 3,
    Returned = 4,
    Approved = 5,
    ReadyForExecution = 6,
    InProgress = 7,
    Closed = 8,
    Cancelled = 9
}

public enum InterviewType
{
    InPerson = 1,
    Remote = 2
}
