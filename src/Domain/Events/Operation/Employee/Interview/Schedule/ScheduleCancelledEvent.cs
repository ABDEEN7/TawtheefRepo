using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Employee.Interview.Schedule;

/// <param name="CancelledAppointmentIds">The candidate bookings cancelled together with the schedule.</param>
public sealed record ScheduleCancelledEvent(
    InterviewSchedule Schedule, IReadOnlyCollection<Guid> CancelledAppointmentIds, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
