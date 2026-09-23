namespace Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;

public sealed record MyQuestionBankAssignmentDto(Guid AssignmentId, Guid RequestId, Guid QuestionBankTypeId,
    string QuestionBankTypeNameAr, string QuestionBankTypeNameEn, string? ManagementNameAr, string? ManagementNameEn,
    string? JobTitleNameAr, string? JobTitleNameEn, Guid StatusId, string StatusNameAr, string StatusNameEn,
    int MinimumQuestionCount, int CurrentQuestionCount, DateTime AssignedAt, DateTime? QuestionEntryStartedAt,
    DateTime? QuestionEntryCompletedAt);
