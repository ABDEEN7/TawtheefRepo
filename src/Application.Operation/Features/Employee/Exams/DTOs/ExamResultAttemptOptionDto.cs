namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamResultAttemptOptionDto(
    Guid QuestionRevisionOptionId,
    string? OptionTextAr,
    string? OptionTextEn,
    int DisplayOrder,
    bool IsSelected);
