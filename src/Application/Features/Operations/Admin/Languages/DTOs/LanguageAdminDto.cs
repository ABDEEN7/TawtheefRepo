namespace Tawtheef.Application.Features.Operations.Admin.Languages.DTOs;

public sealed record LanguageAdminDto
{
    public Guid Id { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public string? DescriptionAr { get; init; }
    public string? DescriptionEn { get; init; }
    public bool IsActive { get; init; }
}
