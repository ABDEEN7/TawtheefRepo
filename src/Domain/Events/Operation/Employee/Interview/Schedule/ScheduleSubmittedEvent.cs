using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Employee.Interview.Schedule;

public sealed record ScheduleSubmittedEvent(InterviewSchedule Schedule, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
