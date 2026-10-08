namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamQuestionGenerationAttemptPartData(
    Guid Id,
    Guid ExamPartId,
    Guid StatusId,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    int? DurationMinutesSnapshot,
    DateTime? EffectiveDeadlineAt,
    int? LastQuestionNo);
