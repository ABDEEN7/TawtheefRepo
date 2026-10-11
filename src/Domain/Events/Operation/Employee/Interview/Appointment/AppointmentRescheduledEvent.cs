using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Employee.Interview.Appointment;

/// <summary>A candidate's booking moved: <paramref name="OldAppointment"/> is now Rescheduled and
/// <paramref name="NewAppointment"/> holds the candidate.</summary>
public sealed record AppointmentRescheduledEvent(
    InterviewAppointment OldAppointment, InterviewAppointment NewAppointment, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
