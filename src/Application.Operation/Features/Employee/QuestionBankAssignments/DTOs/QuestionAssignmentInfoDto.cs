namespace Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;

public sealed record QuestionAssignmentInfoDto(Guid Id, Guid StatusId, string StatusNameAr, string StatusNameEn,
    int MinimumQuestionCount, string? Notes, DateTime AssignedAt, DateTime? QuestionEntryStartedAt,
    DateTime? QuestionEntryCompletedAt);
