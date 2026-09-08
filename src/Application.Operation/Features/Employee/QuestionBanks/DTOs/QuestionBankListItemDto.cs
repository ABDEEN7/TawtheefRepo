namespace Application.Operation.Features.Employee.QuestionBanks.DTOs;

public sealed record QuestionBankListItemDto
{
    public Guid Id { get; init; }
    public Guid QuestionBankTypeId { get; init; }
    public required string QuestionBankTypeNameAr { get; init; }
    public required string QuestionBankTypeNameEn { get; init; }
    public Guid? ManagementId { get; init; }
    public string? ManagementNameAr { get; init; }
    public string? ManagementNameEn { get; init; }
    public Guid? JobTitleId { get; init; }
    public string? JobTitleNameAr { get; init; }
    public string? JobTitleNameEn { get; init; }
    public Guid? StageId { get; init; }
    public string? StageNameAr { get; init; }
    public string? StageNameEn { get; init; }
    public Guid? CurrentApprovedVersionId { get; init; }
    public int? CurrentVersionNo { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime? UpdatedDate { get; init; }
}
