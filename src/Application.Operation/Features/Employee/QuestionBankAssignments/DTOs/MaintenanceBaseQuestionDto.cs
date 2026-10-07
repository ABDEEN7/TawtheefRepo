namespace Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;

public sealed record MaintenanceBaseQuestionDto(
    Guid QuestionId, Guid RevisionId, int RevisionNo, Guid QuestionTypeId,
    string QuestionTypeNameAr, string QuestionTypeNameEn, Guid DifficultyLevelId,
    string DifficultyNameAr, string DifficultyNameEn, string? QuestionTextAr,
    string? QuestionTextEn, string? ExplanationAr, string? ExplanationEn,
    Guid? ResourceId, string? ImageUrl, IReadOnlyCollection<QuestionOptionDto> Options,
    string Ownership, Guid? RequestItemId);
