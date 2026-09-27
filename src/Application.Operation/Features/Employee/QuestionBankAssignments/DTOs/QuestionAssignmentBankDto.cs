namespace Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;

public sealed record QuestionAssignmentBankDto(Guid QuestionBankTypeId, string QuestionBankTypeNameAr,
    string QuestionBankTypeNameEn, string? ManagementNameAr, string? ManagementNameEn,
    string? JobTitleNameAr, string? JobTitleNameEn);
