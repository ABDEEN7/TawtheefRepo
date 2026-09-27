namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record TimeoutCandidateExamPartResponse(
    Guid TestAttemptId,
    TimedOutCandidateExamPartDto TimedOutPart,
    string Outcome,
    bool HasNextPart,
    StartedCandidateExamPartDto? NextPart,
    Guid AttemptStatusId,
    bool IsExamEnded,
    bool WasChanged);
