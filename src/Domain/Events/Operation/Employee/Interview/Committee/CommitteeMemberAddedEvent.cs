using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Employee.Interview.Committee;

public sealed record CommitteeMemberAddedEvent(InterviewCommittee Committee, InterviewCommitteeMember Member, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
