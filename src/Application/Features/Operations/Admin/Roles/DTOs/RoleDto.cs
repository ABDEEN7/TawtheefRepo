namespace Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;

public sealed record RoleDto
{
    public Guid Id { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public string? DescriptionAr { get; init; }
    public string? DescriptionEn { get; init; }
    public bool IsSystemRole { get; init; }
    public required string SystemName { get; init; }
    public IReadOnlyCollection<string> Permissions { get; init; } = Array.Empty<string>();
    public IReadOnlyCollection<string> PermissionNames { get; init; } = Array.Empty<string>();
}
