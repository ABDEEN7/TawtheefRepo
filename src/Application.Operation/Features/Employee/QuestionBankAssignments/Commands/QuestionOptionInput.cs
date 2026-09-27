namespace Application.Operation.Features.Employee.QuestionBankAssignments.Commands;

public sealed record QuestionOptionInput(
    string? OptionTextAr,
    string? OptionTextEn,
    bool IsCorrect,
    int DisplayOrder);
