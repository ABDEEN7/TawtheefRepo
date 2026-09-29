namespace Application.Operation.Features.Employee.QuestionBankRequests.DTOs;

public sealed record QuestionBankRequestReviewDto
{
    public Guid RequestId { get; init; }
    public Guid QuestionBankId { get; init; }
    public Guid RequestTypeId { get; init; }
    public required string RequestTypeNameAr { get; init; }
    public required string RequestTypeNameEn { get; init; }
    public Guid RequestStatusId { get; init; }
    public required string RequestStatusNameAr { get; init; }
    public required string RequestStatusNameEn { get; init; }
    public int CurrentReviewRound { get; init; }
    public Guid QuestionBankTypeId { get; init; }
    public required string QuestionBankTypeNameAr { get; init; }
    public required string QuestionBankTypeNameEn { get; init; }
    public string? ManagementNameAr { get; init; }
    public string? ManagementNameEn { get; init; }
    public string? JobTitleNameAr { get; init; }
    public string? JobTitleNameEn { get; init; }
    public IReadOnlyCollection<QuestionBankReviewItemDto> Items { get; init; } = [];
}
