namespace Tawtheef.Application.Features.Operations.Admin.Users.DTOs;

public sealed record UserRoleAssignmentDto
{
    public Guid UserId { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public bool IsBlocked { get; init; }
    public DateTime? LastLoginDate { get; init; }
    public IReadOnlyCollection<RoleSummaryDto> Roles { get; init; } = Array.Empty<RoleSummaryDto>();
    public IReadOnlyCollection<Guid> AssignedRoleIds { get; init; } = Array.Empty<Guid>();
}
