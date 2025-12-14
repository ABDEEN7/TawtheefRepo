namespace Tawtheef.Application.Features.Operations.Admin.Users.DTOs;

public sealed class RoleSummaryDto
{
    public Guid Id { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
}
