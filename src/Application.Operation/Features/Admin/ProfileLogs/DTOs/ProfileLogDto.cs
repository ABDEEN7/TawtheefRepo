using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Admin.ProfileLogs.DTOs;

public sealed record ProfileLogDto
{
    public Guid Id { get; init; }
    public Guid UserProfileId { get; init; }
    public required string Source { get; init; }
    public required string ActionType { get; init; }
    public string? Section { get; init; }
    public string? Notes { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public Guid? EntityId { get; init; }
    public Guid? AttachmentId { get; init; }
    public ReviewStatus? ReviewStatus { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
}
