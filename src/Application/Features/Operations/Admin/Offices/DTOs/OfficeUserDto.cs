namespace Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;

public sealed record OfficeUserDto
{
    public Guid Id { get; init; }
    public string FullNameAr { get; init; } = string.Empty;
    public string FullNameEn { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsBlocked { get; init; }
}
