namespace Application.Recruitment.Features.CandidateExams.Models;

public sealed record CandidateExamResumeResult(
    Guid TestAttemptId,
    Guid TestAttemptPartId,
    Guid ActiveSessionId,
    DateTimeOffset EffectiveDeadlineAt,
    int CurrentQuestionNumber,
    bool AllowPreviousQuestion,
    bool WasChanged);
