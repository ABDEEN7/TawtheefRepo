using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

public sealed record CommitteeMemberAddedEvent(InterviewCommittee Committee, InterviewCommitteeMember Member, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
