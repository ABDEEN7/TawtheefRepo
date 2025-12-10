namespace Tawtheef.Application.Features.Operations.Admin.Users.DTOs;

public sealed record UserListItemDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public DateTime? LastLoginDate { get; init; }
    public bool IsBlocked { get; init; }
    public IReadOnlyCollection<string> Roles { get; init; } = Array.Empty<string>();
}
