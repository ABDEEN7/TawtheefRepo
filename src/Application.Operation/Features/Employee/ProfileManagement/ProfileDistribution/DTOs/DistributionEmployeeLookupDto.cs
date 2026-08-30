namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;

public sealed class DistributionEmployeeLookupDto
{
    public Guid EmployeeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DistributionEmployeeAvailability Availability { get; init; }
}
