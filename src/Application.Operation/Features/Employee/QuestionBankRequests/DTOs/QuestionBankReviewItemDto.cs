namespace Application.Operation.Features.Employee.QuestionBankRequests.DTOs;

public sealed record QuestionBankReviewItemDto
{
    public Guid RequestItemId { get; init; }
    public Guid QuestionId { get; init; }
    public Guid QuestionBankAssignmentId { get; init; }
    public string? EmployeeNameAr { get; init; }
    public string? EmployeeNameEn { get; init; }
    public Guid ChangeTypeId { get; init; }
    public Guid ItemStatusId { get; init; }
    public Guid CurrentProposedRevisionId { get; init; }
    public Guid QuestionTypeId { get; init; }
    public required string QuestionTypeNameAr { get; init; }
    public required string QuestionTypeNameEn { get; init; }
    public Guid DifficultyLevelId { get; init; }
    public required string DifficultyNameAr { get; init; }
    public required string DifficultyNameEn { get; init; }
    public string? QuestionTextAr { get; init; }
    public string? QuestionTextEn { get; init; }
    public string? ExplanationAr { get; init; }
    public string? ExplanationEn { get; init; }
    public string? ImageUrl { get; init; }
    public IReadOnlyCollection<QuestionBankReviewOptionDto> Options { get; init; } = [];
}
