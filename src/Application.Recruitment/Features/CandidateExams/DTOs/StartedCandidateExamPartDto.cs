namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record StartedCandidateExamPartDto(
    Guid TestAttemptPartId,
    int PartNo,
    DateTime StartedAt,
    int DurationMinutes,
    DateTime EffectiveDeadlineAt,
    int CurrentQuestionNumber,
    int TotalQuestionsInPart);
