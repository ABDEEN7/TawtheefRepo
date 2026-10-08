namespace Application.Operation.Features.Employee.QuestionBankRequests.DTOs;

public sealed record QuestionBankReviewOptionDto
{
    public Guid Id { get; init; }
    public string? OptionTextAr { get; init; }
    public string? OptionTextEn { get; init; }
    public bool IsCorrect { get; init; }
    public int DisplayOrder { get; init; }
}
