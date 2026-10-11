using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Employee.Interview.Appointment;

/// <summary>Staff sent the candidate their interview invitation (first send) or a reminder (later sends).</summary>
public sealed record AppointmentNotificationSentEvent(
    InterviewAppointment Appointment, bool IsReminder, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
