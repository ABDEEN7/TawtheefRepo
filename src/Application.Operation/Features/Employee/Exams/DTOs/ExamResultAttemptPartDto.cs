namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamResultAttemptPartDto(
    Guid TestAttemptPartId,
    int PartNo,
    ExamResultLookupDto Status,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    int? DurationMinutesSnapshot,
    DateTime? EffectiveDeadlineAt,
    IReadOnlyList<ExamResultAttemptQuestionDto> Questions);
