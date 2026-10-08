namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record StartCandidateExamResponse(
    Guid TestSessionCandidateId,
    Guid TestAttemptId,
    Guid TestAttemptPartId,
    Guid ExamId,
    Guid ActiveSessionId,
    int CurrentPartNumber,
    int CurrentQuestionNumber,
    DateTimeOffset StartedAt,
    DateTimeOffset EffectiveDeadlineAt,
    int DurationMinutes,
    bool AllowPreviousQuestion);
