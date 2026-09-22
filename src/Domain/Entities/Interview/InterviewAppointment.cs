using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewAppointment), Schema = Schemas.Interview)]
public class InterviewAppointment : EventEntity
{
    public Guid InterviewScheduleId { get; set; }
    public InterviewSchedule? InterviewSchedule { get; set; }

    // NULL = an open, unassigned slot ("Held"). Set once a candidate is assigned.
    public Guid? InvitationId { get; set; }
    public Invitation? Invitation { get; set; }

    public Guid InterviewCommitteeId { get; set; }
    public InterviewCommittee? InterviewCommittee { get; set; }

    public InterviewType InterviewType { get; set; }

    public Guid? RoomId { get; set; }
    public Room? Room { get; set; }
    public string? RemoteMeetingUrl { get; set; }
    public string? RemoteMeetingInstructions { get; set; }

    public DateTime StartAt { get; set; } // NOOOOOOTE : must use it for check conflict and also must check the start and end time for room in exam slot
    public DateTime EndAt { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Held;

    // Stays NULL until the interview day. Never encode attendance in Status.
    public AttendanceStatus? AttendanceStatus { get; set; }

    public DateTime? ActualStartAt { get; set; }
    public DateTime? ActualEndAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public Guid? RescheduledFromAppointmentId { get; set; }
    public InterviewAppointment? RescheduledFromAppointment { get; set; }
    public string? RescheduleReason { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? InvitationSentAt { get; set; }
    public DateTime? LastReminderSentAt { get; set; }
    public int ReminderCount { get; set; }

    public static InterviewAppointment Create(
        Guid interviewScheduleId, Guid interviewCommitteeId, InterviewType interviewType,
        Guid? roomId, string? remoteMeetingUrl, string? remoteMeetingInstructions,
        DateTime startAt, DateTime endAt, Guid? rescheduledFromAppointmentId = null)
    {
        return new InterviewAppointment
        {
            InterviewScheduleId = interviewScheduleId,
            InterviewCommitteeId = interviewCommitteeId,
            InterviewType = interviewType,
            RoomId = roomId,
            RemoteMeetingUrl = remoteMeetingUrl,
            RemoteMeetingInstructions = remoteMeetingInstructions,
            StartAt = startAt,
            EndAt = endAt,
            Status = AppointmentStatus.Held,
            InvitationId = null,
            RescheduledFromAppointmentId = rescheduledFromAppointmentId
        };
    }

    // Blocks edits once the interview has actually started or finished — enforced for
    // real in this step, unlike the deferred equivalent in step 5.
    // if there special case for rescheduled or cancelled appointments, we can add it later
    public Result EnsureEditable() =>
        ActualStartAt is null && Status is not (AppointmentStatus.InInterview
            or AppointmentStatus.UnderEvaluation or AppointmentStatus.Completed or AppointmentStatus.Closed)
            ? Result.Ok()
            : Result.Fail(new Error(ErrorsCodes.InterviewAppointmentNotEditable));

    public Result AssignCandidate(Guid invitationId)
    {
        var editable = EnsureEditable();
        if (editable.IsFailed)
            return editable;

        if (Status != AppointmentStatus.Held)
            return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentNotHeld));

        InvitationId = invitationId;
        Status = AppointmentStatus.Scheduled;
        return Result.Ok();
    }

    public Result UnassignCandidate()
    {
        var editable = EnsureEditable();
        if (editable.IsFailed)
            return editable;

        if (Status != AppointmentStatus.Scheduled)
            return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentNotScheduled));

        InvitationId = null;
        Status = AppointmentStatus.Held;
        return Result.Ok();
    }

    // Marks THIS row superseded; the handler creates the new replacement row and links
    // it back via RescheduledFromAppointmentId. AttendanceStatus is left untouched.
    // allowCompleted is a narrow, caller-verified exception for the Final Review corrective-reschedule
    // scenario only (a Completed appointment whose schedule's result report is still UnderReview) -
    // every other EnsureEditable() guard (AssignCandidate/UnassignCandidate/Cancel) is untouched.
    public Result MarkRescheduled(string reason, bool allowCompleted = false)
    {
        var editable = EnsureEditable();
        if (editable.IsFailed && !(allowCompleted && Status == AppointmentStatus.Completed))
            return editable;

        Status = AppointmentStatus.Rescheduled;
        RescheduleReason = reason;
        return Result.Ok();
    }

    public Result Cancel(string reason)
    {
        var editable = EnsureEditable();
        if (editable.IsFailed)
            return editable;

        Status = AppointmentStatus.Cancelled;
        CancellationReason = reason;
        return Result.Ok();
    }

    public Result RecordAttendance(AttendanceStatus attendanceStatus)
    {
        AttendanceStatus = attendanceStatus;
        return Result.Ok();
    }

    // BRD: can't start before attendance is recorded, and a NoShow candidate never gets
    // an evaluation form opened at all.
    public Result StartInterview()
    {
        if (Status != AppointmentStatus.Scheduled)
            return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentNotScheduled));
        if (AttendanceStatus != Interview.AttendanceStatus.Present)
            return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentAttendanceNotRecorded));

        Status = AppointmentStatus.InInterview;
        ActualStartAt = DateTime.UtcNow;
        return Result.Ok();
    }

    // Triggered by the first member draft save.
    public Result MarkUnderEvaluation()
    {
        if (Status != AppointmentStatus.InInterview)
            return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentNotInInterview));

        Status = AppointmentStatus.UnderEvaluation;
        return Result.Ok();
    }

    // Auto-triggered once every required member has submitted (the committee quorum).
    public Result CompleteEvaluation()
    {
        if (Status != AppointmentStatus.UnderEvaluation)
            return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentNotUnderEvaluation));

        Status = AppointmentStatus.Completed;
        ActualEndAt = DateTime.UtcNow;
        return Result.Ok();
    }

    // A distinct, later, chair-triggered step from Completed -- "closing freezes
    // evaluations and triggers calculation" (step 8, once it exists).
    public Result Close()
    {
        if (Status != AppointmentStatus.Completed)
            return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentNotCompleted));

        Status = AppointmentStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        return Result.Ok();
    }

    // Notification gating (schedule.md): first send sets InvitationSentAt: every later
    // send is a reminder and bumps LastReminderSentAt/ReminderCount instead. The caller
    // must verify the parent InterviewSchedule.Status is Approved or Closed beforehand -
    // a cross-aggregate check this entity cannot perform itself.
    public Result RecordNotificationSent()
    {
        if (InvitationSentAt is null)
            InvitationSentAt = DateTime.UtcNow;
        else
        {
            LastReminderSentAt = DateTime.UtcNow;
            ReminderCount++;
        }

        return Result.Ok();
    }
}

public enum AppointmentStatus
{
    Held = 1,
    Scheduled = 2,
    InInterview = 3,
    UnderEvaluation = 4,
    Completed = 5,
    Closed = 6,
    Rescheduled = 7,
    Cancelled = 8
}

public enum AttendanceStatus
{
    Present = 1,
    NoShow = 2,
    Withdrew = 3,
    Late = 4
}
