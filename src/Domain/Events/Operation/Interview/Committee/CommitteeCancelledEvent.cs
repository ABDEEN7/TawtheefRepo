using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Interview.Committee;

public sealed record CommitteeCancelledEvent(InterviewCommittee Committee, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
