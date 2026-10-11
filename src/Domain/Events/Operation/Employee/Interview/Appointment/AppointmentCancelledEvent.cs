using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Employee.Interview.Appointment;

/// <summary>A booking was cancelled - on its own, or because its whole schedule was cancelled.</summary>
public sealed record AppointmentCancelledEvent(InterviewAppointment Appointment, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
