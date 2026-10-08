namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record SubmitCandidateExamPartResponse(
    Guid TestAttemptId,
    CompletedCandidateExamPartDto CompletedPart,
    string Outcome,
    bool HasNextPart,
    StartedCandidateExamPartDto? NextPart,
    Guid AttemptStatusId,
    bool IsExamEnded,
    bool AllowPreviousQuestion,
    bool WasChanged);
