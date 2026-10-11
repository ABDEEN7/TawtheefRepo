namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record StartedCandidateExamPartDto(
    Guid TestAttemptPartId,
    int PartNo,
    DateTimeOffset StartedAt,
    int DurationMinutes,
    DateTimeOffset EffectiveDeadlineAt,
    int CurrentQuestionNumber,
    int TotalQuestionsInPart);
