using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Profile.DTOs;

public sealed record ProfileChangeRequestDto
{
    public Guid Id { get; init; }
    public ProfileSection Section { get; init; }
    public ProfileChangeAction Action { get; init; }
    public ProfileChangeRequestStatus Status { get; init; }
    public string TargetKey { get; init; } = string.Empty;
    public string? FieldPath { get; init; }
    public string? EntityName { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public DateTimeOffset RequestedAtUtc { get; init; }
    public DateTimeOffset? ReviewedAtUtc { get; init; }
    public string? ReviewerNote { get; init; }
}
