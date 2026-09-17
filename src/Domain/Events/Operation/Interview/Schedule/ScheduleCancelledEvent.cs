using System;
using System.Collections.Generic;
using System.Text;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Interview.Schedule;

public sealed record ScheduleCancelledEvent(InterviewSchedule Schedule, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
