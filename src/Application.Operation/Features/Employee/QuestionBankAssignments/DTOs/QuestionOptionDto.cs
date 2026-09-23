namespace Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;

public sealed record QuestionOptionDto(Guid? Id, string? OptionTextAr, string? OptionTextEn, bool IsCorrect, int DisplayOrder);
