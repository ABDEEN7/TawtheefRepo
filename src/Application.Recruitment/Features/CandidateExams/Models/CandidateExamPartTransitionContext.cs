using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamPartTransitionContext(
    TestAttempt Attempt,
    TestSessionCandidate Candidate,
    Guid SessionStatusId,
    Guid SlotStatusId,
    bool AllowPreviousQuestion,
    IReadOnlyList<CandidateExamPartTransitionData> Parts,
    int RequestedIndex)
{
    public CandidateExamPartTransitionData Requested =>
        Parts[RequestedIndex];

    public CandidateExamPartTransitionData? Next =>
        RequestedIndex + 1 < Parts.Count ? Parts[RequestedIndex + 1] : null;

    public bool ActiveSessionMatches(Guid activeSessionId) =>
        Attempt.ActiveSessionId == activeSessionId;

    public bool IsTerminalReplay =>
        Attempt.ActiveSessionId is null
        && (Attempt.StatusId == TestAttemptStatusIds.Completed || Attempt.StatusId == TestAttemptStatusIds.TimedOut)
        && Candidate.StatusId == TestSessionCandidateStatusIds.Completed;

    public bool IsRequestedPartTheOnlyRunningPart() =>
        Requested.Part.StatusId == TestAttemptPartStatusIds.InProgress
        && Parts.Count(x => x.Part.StatusId == TestAttemptPartStatusIds.InProgress) == 1;
}
