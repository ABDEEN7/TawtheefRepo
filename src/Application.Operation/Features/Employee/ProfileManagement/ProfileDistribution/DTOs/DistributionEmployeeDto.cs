namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;

public sealed class DistributionEmployeeDto
{
    public Guid EmployeeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int TotalAssigned { get; init; }
    public int Completed { get; init; }
    public int InReview { get; init; }
    public bool IsActive { get; init; }
    public DistributionEmployeeAvailability Availability { get; init; }
}

public enum DistributionEmployeeAvailability
{
    Available = 1,
    OnLeave = 2,
    Suspended = 3,
    Inactive = 4
}
