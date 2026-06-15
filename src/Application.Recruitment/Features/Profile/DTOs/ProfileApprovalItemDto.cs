using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Profile.DTOs;

public record ProfileApprovalItemDto
{
    public Guid ReviewItemId { get; init; }
    public ReviewTargetType TargetType { get; init; }
    public ReviewStatus Status { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Note { get; init; }
    public Guid? ResourceId { get; init; }
    public string? ResourceUrl { get; set; }
    public string? FieldPath { get; init; }
    public Guid? EntityId { get; init; }
    public string? EntityName { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public DateTimeOffset? ReviewedAtUtc { get; init; }
}
