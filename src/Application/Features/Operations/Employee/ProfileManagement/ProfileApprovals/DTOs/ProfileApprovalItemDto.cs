using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;

public record ProfileApprovalItemDto
{
    public Guid ReviewItemId { get; init; }
    public ReviewTargetType TargetType { get; init; }
    public ReviewStatus Status { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Note { get; init; }
    public Guid? ResourceId { get; init; }
    public string? ResourceUrl { get; set; }
    public Guid? EntityId { get; init; }
    public string? EntityName { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public int Version { get; init; }
    public int? ApprovedAtVersion { get; init; }
    public DateTime? ReviewedAtUtc { get; init; }
}
