namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamResultAttemptQuestionDto(
    Guid TestAttemptQuestionId,
    int OrderNo,
    Guid QuestionRevisionId,
    string? QuestionTextAr,
    string? QuestionTextEn,
    ExamResultLookupDto QuestionType,
    ExamResultLookupDto? Category,
    Guid? SelectedQuestionRevisionOptionId,
    DateTime? SavedAt,
    bool IsAnswered,
    bool? IsCorrect,
    IReadOnlyList<ExamResultAttemptOptionDto> Options);
