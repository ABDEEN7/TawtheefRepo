using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

public sealed record CommitteeMemberRemovedEvent(InterviewCommittee Committee, InterviewCommitteeMember Member, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
