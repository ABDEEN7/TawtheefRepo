using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

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
    public DateTime RequestedAtUtc { get; init; }
    public DateTime? ReviewedAtUtc { get; init; }
    public string? ReviewerNote { get; init; }
}
