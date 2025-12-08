namespace Tawtheef.Application.Features.Operations.ProfileDistribution.DTOs;

public sealed class DistributionResultDto
{
    public int AssignedCount { get; init; }
    public IReadOnlyList<DistributionEmployeeDto> Employees { get; init; } = Array.Empty<DistributionEmployeeDto>();
    public IReadOnlyList<DistributionProfileDto> Profiles { get; init; } = Array.Empty<DistributionProfileDto>();
}
