using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Employee.Interview.Committee;

public sealed record CommitteeApprovedEvent(InterviewCommittee Committee, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
