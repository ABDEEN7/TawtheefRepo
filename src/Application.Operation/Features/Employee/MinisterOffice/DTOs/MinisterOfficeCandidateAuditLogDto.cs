namespace Application.Operation.Features.Employee.MinisterOffice.DTOs;

public sealed class MinisterOfficeCandidateAuditLogDto
{
    public Guid Id { get; init; }
    public string Action { get; init; } = default!;
    public string? Details { get; init; }
    public string? Qid { get; init; }
    public string OperatorNameEn { get; init; } = default!;
    public string OperatorNameAr { get; init; } = default!;
    public DateTimeOffset CreatedDate { get; init; }
}
