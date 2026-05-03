using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Admin.SystemAdminLogs.DTOs;

public sealed record SystemAdminLogDto
{
    public required Guid Id { get; init; }
    public required Guid UserProfileId { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public string? UserProfileOwnerName { get; init; }
    public required string Source { get; init; }
    public required string ActionType { get; init; }
    public string? Section { get; init; }
    public string? Notes { get; init; }
    public Guid? EntityId { get; init; }
    public Guid? AttachmentId { get; init; }
    public ReviewStatus? ReviewStatus { get; init; }
    public required DateTimeOffset CreatedDate { get; init; }
}
