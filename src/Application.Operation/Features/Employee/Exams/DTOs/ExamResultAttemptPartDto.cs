namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamResultAttemptPartDto(
    Guid TestAttemptPartId,
    int PartNo,
    ExamResultLookupDto Status,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    int? DurationMinutesSnapshot,
    DateTimeOffset? EffectiveDeadlineAt,
    IReadOnlyList<ExamResultAttemptQuestionDto> Questions);
