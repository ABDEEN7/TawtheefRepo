namespace Application.Operation.Features.Employee.QuestionBankAssignments.Commands;

public sealed record QuestionInput(
    Guid QuestionTypeId,
    Guid DifficultyLevelId,
    string? QuestionTextAr,
    string? QuestionTextEn,
    string? ExplanationAr,
    string? ExplanationEn,
    IReadOnlyCollection<QuestionOptionInput> Options);
