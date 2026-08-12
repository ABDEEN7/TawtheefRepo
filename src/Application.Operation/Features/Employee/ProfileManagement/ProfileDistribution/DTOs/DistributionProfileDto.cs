using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;

public sealed class DistributionProfileDto
{
    public Guid ProfileId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string TargetEntity { get; set; } = string.Empty;
    public UserProfileStatus Status { get; set; }
    public Guid? AssignedEmployeeId { get; set; } = Guid.Empty;
    public string? AssignedEmployeeName { get; set; } = string.Empty;
    public bool HasOtherSpecialization { get; set; }
    public bool HasOtherUniversity { get; set; }
    public bool IsMinisterOfficeCandidate { get; set; }
    public DateTimeOffset SubmittedAtUtc { get; set; }
}
