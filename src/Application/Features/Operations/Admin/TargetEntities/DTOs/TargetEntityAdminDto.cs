namespace Tawtheef.Application.Features.Operations.Admin.TargetEntities.DTOs;

public sealed record TargetEntityAdminDto
{
    public Guid Id { get; init; }
    public required string BackendName { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public string? DescriptionAr { get; init; }
    public string? DescriptionEn { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}
