namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;

public sealed class DistributionResultDto
{
    public int AssignedCount { get; init; }
    public IReadOnlyList<DistributionEmployeeDto> Employees { get; init; } = [];
    public IReadOnlyList<DistributionProfileDto> Profiles { get; init; } = [];
}
