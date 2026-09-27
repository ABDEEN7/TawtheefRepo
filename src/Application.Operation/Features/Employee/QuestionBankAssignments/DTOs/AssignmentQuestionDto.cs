namespace Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;

public sealed record AssignmentQuestionDto(Guid ItemId, Guid QuestionId, Guid RevisionId, int RevisionNo,
    Guid QuestionTypeId, string QuestionTypeNameAr, string QuestionTypeNameEn, Guid DifficultyLevelId,
    string DifficultyNameAr, string DifficultyNameEn, string? QuestionTextAr, string? QuestionTextEn,
    string? ExplanationAr, string? ExplanationEn, Guid StatusId, string StatusNameAr, string StatusNameEn,
    IReadOnlyCollection<QuestionOptionDto> Options);
