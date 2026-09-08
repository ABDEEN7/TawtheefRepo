using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
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

    // NOOOOOOTE  : Must add a Room entity and FK after merge  the branches just store the ID.
    // Guid for now; add the FK later.
    public Guid? RoomId { get; set; }
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
