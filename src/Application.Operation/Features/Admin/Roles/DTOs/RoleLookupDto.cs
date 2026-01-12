namespace Application.Operation.Features.Admin.Roles.DTOs;

public sealed record RoleLookupDto
{
    public Guid Id { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public required string SystemName { get; init; }
    public bool IsSystemRole { get; init; }
}
