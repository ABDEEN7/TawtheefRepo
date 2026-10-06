namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record CandidateExamActiveExecutionResponse(
    Guid TestAttemptId,
    Guid TestAttemptPartId,
    Guid ActiveSessionId,
    DateTime EffectiveDeadlineAt,
    int CurrentQuestionNumber,
    bool AllowPreviousQuestion);
