namespace Application.Operation.Features.Admin.Users.DTOs;

public sealed record UserRoleAssignmentDto
{
    public Guid UserId { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public bool IsBlocked { get; init; }
    public DateTimeOffset? LastLoginDate { get; init; }
    public IReadOnlyCollection<RoleSummaryDto> Roles { get; init; } = [];
    public IReadOnlyCollection<Guid> AssignedRoleIds { get; init; } = [];
}
