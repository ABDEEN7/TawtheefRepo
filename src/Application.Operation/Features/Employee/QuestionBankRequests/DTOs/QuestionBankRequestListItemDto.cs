namespace Application.Operation.Features.Employee.QuestionBankRequests.DTOs;

public sealed record QuestionBankRequestListItemDto
{
    public Guid Id { get; init; }
    public Guid QuestionBankId { get; init; }
    public Guid RequestTypeId { get; init; }
    public required string RequestTypeNameAr { get; init; }
    public required string RequestTypeNameEn { get; init; }
    public Guid StatusId { get; init; }
    public required string StatusNameAr { get; init; }
    public required string StatusNameEn { get; init; }
    public Guid QuestionBankTypeId { get; init; }
    public required string QuestionBankTypeNameAr { get; init; }
    public required string QuestionBankTypeNameEn { get; init; }
    public Guid? ManagementId { get; init; }
    public string? ManagementNameAr { get; init; }
    public string? ManagementNameEn { get; init; }
    public Guid? JobTitleId { get; init; }
    public string? JobTitleNameAr { get; init; }
    public string? JobTitleNameEn { get; init; }
    public Guid SubmittedById { get; init; }
    public string? SubmittedByNameAr { get; init; }
    public string? SubmittedByNameEn { get; init; }
    public DateTime SubmittedAt { get; init; }
    public int CurrentReviewRound { get; init; }
}
