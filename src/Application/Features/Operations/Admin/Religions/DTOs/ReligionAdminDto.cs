namespace Tawtheef.Application.Features.Operations.Admin.Religions.DTOs;

public sealed record ReligionAdminDto
{
    public Guid Id { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public string? DescriptionAr { get; init; }
    public string? DescriptionEn { get; init; }
    public bool IsActive { get; init; }
}
