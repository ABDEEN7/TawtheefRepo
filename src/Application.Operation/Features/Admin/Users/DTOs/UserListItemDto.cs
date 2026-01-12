namespace Application.Operation.Features.Admin.Users.DTOs;

public sealed record UserListItemDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public DateTimeOffset? LastLoginDate { get; init; }
    public bool IsBlocked { get; init; }
    public IReadOnlyCollection<RoleSummaryDto> Roles { get; init; } = [];
    public List<string> RoleNames { get; set; } = [];
}
