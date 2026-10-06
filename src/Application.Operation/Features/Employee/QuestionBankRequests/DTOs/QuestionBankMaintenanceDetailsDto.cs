namespace Application.Operation.Features.Employee.QuestionBankRequests.DTOs;

public sealed record QuestionBankMaintenanceDetailsDto
{
    public Guid QuestionBankId { get; init; }
    public Guid QuestionBankTypeId { get; init; }
    public required string QuestionBankTypeNameAr { get; init; }
    public required string QuestionBankTypeNameEn { get; init; }
    public Guid? ManagementId { get; init; }
    public string? ManagementNameAr { get; init; }
    public string? ManagementNameEn { get; init; }
    public Guid? JobTitleId { get; init; }
    public string? JobTitleNameAr { get; init; }
    public string? JobTitleNameEn { get; init; }
    public Guid? CurrentApprovedVersionId { get; init; }
    public int? CurrentVersionNo { get; init; }
    public bool IsActive { get; init; }
}
