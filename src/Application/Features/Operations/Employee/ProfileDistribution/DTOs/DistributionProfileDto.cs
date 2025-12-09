using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileDistribution.DTOs;

public sealed class DistributionProfileDto
{
    public Guid ProfileId { get; init; }
    public string CandidateName { get; init; } = string.Empty;
    public string Specialization { get; init; } = string.Empty;
    public string TargetEntity { get; init; } = string.Empty;
    public UserProfileStatus Status { get; init; }
    public Guid? AssignedEmployeeId { get; init; }
    public string? AssignedEmployeeName { get; init; }
    public DateTime SubmittedAtUtc { get; init; }
}
