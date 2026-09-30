using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
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

    // "Live" candidate appointment = has a real candidate (not a Held slot) and wasn't pulled out of the
    // running (not Cancelled/Rescheduled - a Rescheduled row is superseded by its replacement, which is
    // itself live). Single definition shared by result-report generation and the Interview Dashboard;
    // the expression translates to SQL, the compiled form evaluates tracked (unsaved) instances.
    public static readonly Expression<Func<InterviewAppointment, bool>> IsLiveCandidate = a =>
        a.InvitationId != null
        && a.Status != AppointmentStatus.Cancelled
        && a.Status != AppointmentStatus.Rescheduled;

    public static readonly Func<InterviewAppointment, bool> IsLiveCandidateCompiled = IsLiveCandidate.Compile();

    // An open seat in its schedule: a generated slot nobody is assigned to (surplus capacity from
    // creation, or a seat vacated by a reschedule). ScheduleAppointmentPlanner.DistributeCandidates
    // pairs slots and candidates 1:1, so every slot seats exactly one candidate and an open slot is
    // exactly one remaining seat. Shared by the reschedule slot lookup and the reschedule command.
    public static readonly Expression<Func<InterviewAppointment, bool>> IsOpenSlot = a =>
        a.Status == AppointmentStatus.Held && a.InvitationId == null;

    public static readonly Func<InterviewAppointment, bool> IsOpenSlotCompiled = IsOpenSlot.Compile();

    // "Done" for final-review purposes, not just "evaluated" - StartInterview() requires
    // AttendanceStatus == Present, so a NoShow/Withdrew appointment can NEVER reach Completed on its
    // own. Without this, a schedule with even one such candidate would never generate a report at all,
    // even though that candidate has clearly "reached the point where their evaluation/review is
    // available" (there's simply nothing more to wait for). ApplyAttendanceOutcome now closes those
    // appointments outright; the attendance check still covers rows recorded before it did. A late
    // candidate is Present and follows the normal flow.
    [NotMapped]
    public bool IsReadyForReview =>
        Status is AppointmentStatus.Completed or AppointmentStatus.Closed
        || AttendanceStatus is Interview.AttendanceStatus.NoShow or Interview.AttendanceStatus.Withdrew;

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

    // Marks THIS row superseded; the handler moves the candidate into the replacement row and links
    // it back via RescheduledFromAppointmentId. AttendanceStatus is left untouched.
    // allowCompleted is a narrow, caller-verified exception for the Final Review corrective-reschedule
    // scenario only (a Completed appointment whose schedule's result report is still UnderReview) -
    // every other EnsureEditable() guard (AssignCandidate/UnassignCandidate/Cancel) is untouched.
    public Result MarkRescheduled(string reason, bool allowCompleted = false)
    {
        var editable = EnsureEditable();
        if (editable.IsFailed && !(allowCompleted && Status == AppointmentStatus.Completed))
            return editable;

        // Only the candidate's live booking can move. EnsureEditable lets an already-Rescheduled or
        // Cancelled row through (it only guards started/finished interviews), and moving one of those
        // would give the candidate a second live appointment next to the replacement they already have.
        if (Status is not (AppointmentStatus.Scheduled or AppointmentStatus.Completed))
            return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentNotScheduled));

        Status = AppointmentStatus.Rescheduled;
        RescheduleReason = reason;
        return Result.Ok();
    }

    // The receiving side of a reschedule: this open slot takes over the candidate of the row that was
    // just marked Rescheduled, and links back to it.
    public Result AcceptRescheduledCandidate(Guid invitationId, Guid rescheduledFromAppointmentId)
    {
        var assignResult = AssignCandidate(invitationId);
        if (assignResult.IsFailed)
            return assignResult;

        RescheduledFromAppointmentId = rescheduledFromAppointmentId;
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

    // The single place attendance changes, and with it the closure rules (attendance.md):
    //
    //   outcome   | AttendanceStatus    | Appointment
    //   ----------+---------------------+-----------------------------------------------
    //   Present   | Present             | normal flow (StartInterview is a separate step)
    //   NoShow    | NoShow ("Absent")   | Scheduled -> Closed (only before the interview starts)
    //   Withdrew  | Withdrew            | Scheduled / InInterview / UnderEvaluation -> Closed
    //   Late      | unchanged (Present) | NOT closed - lateness is a flag (a Late operational issue)
    //
    // Invariant: an appointment is never Scheduled/InInterview/UnderEvaluation while its attendance is
    // NoShow or Withdrew - those outcomes close it in the same call, and are final. The result report
    // scores a closed-by-attendance candidate 0 (InterviewResultCalculationService).
    // The caller decides who may record which outcome: the chair's attendance command only allows
    // Present/NoShow; Withdrew and Late come from operational issues.
    public Result ApplyAttendanceOutcome(AttendanceStatus outcome)
    {
        if (InvitationId is null)
            return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentNotScheduled));

        if (IsClosedByAttendance)
            return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentAttendanceFinal));

        switch (outcome)
        {
            case Interview.AttendanceStatus.Present:
                // Re-confirming Present is harmless; a first Present is only recorded on a Scheduled booking.
                if (Status != AppointmentStatus.Scheduled && AttendanceStatus != Interview.AttendanceStatus.Present)
                    return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentNotScheduled));
                AttendanceStatus = Interview.AttendanceStatus.Present;
                return Result.Ok();

            case Interview.AttendanceStatus.NoShow:
                if (Status != AppointmentStatus.Scheduled || ActualStartAt is not null)
                    return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentAbsentAfterStart));
                return CloseWithAttendance(Interview.AttendanceStatus.NoShow);

            case Interview.AttendanceStatus.Withdrew:
                if (Status is not (AppointmentStatus.Scheduled or AppointmentStatus.InInterview or AppointmentStatus.UnderEvaluation))
                    return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentWithdrawalNotAllowed));
                return CloseWithAttendance(Interview.AttendanceStatus.Withdrew);

            case Interview.AttendanceStatus.Late:
                return Result.Ok();

            default:
                return Result.Fail(new Error(ErrorsCodes.InterviewAppointmentAttendanceStatusNotAllowed));
        }
    }

    // Lateness is a flag, not an attendance value: a Late operational issue on the appointment (attendance
    // stays Present). AttendanceStatus.Late is still honoured for rows recorded before that rule. Queries
    // that project straight to SQL spell the same rule inline.
    public static bool IsLateCandidate(AttendanceStatus? attendanceStatus, IEnumerable<OperationalIssueType> issueTypes) =>
        attendanceStatus == Interview.AttendanceStatus.Late || issueTypes.Contains(OperationalIssueType.Late);

    // Absent/withdrawn: the appointment is over for good - no interview, no reschedule, score 0.
    [NotMapped]
    public bool IsClosedByAttendance =>
        AttendanceStatus is Interview.AttendanceStatus.NoShow or Interview.AttendanceStatus.Withdrew;

    private Result CloseWithAttendance(AttendanceStatus attendanceStatus)
    {
        AttendanceStatus = attendanceStatus;
        Status = AppointmentStatus.Closed;
        ClosedAt = DateTime.UtcNow;
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

    // A distinct, later step from Completed -- "closing freezes evaluations". Chair-triggered, or done
    // by the schedule once its result report is approved. A NoShow/Withdrew candidate never reaches
    // Completed, so anything ready for review (and not already Closed) may close.
    public Result Close()
    {
        if (Status == AppointmentStatus.Closed || !IsReadyForReview)
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
