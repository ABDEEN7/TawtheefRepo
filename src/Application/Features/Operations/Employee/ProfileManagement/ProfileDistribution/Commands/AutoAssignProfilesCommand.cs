using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Commands;

public sealed record AutoAssignProfilesCommand(
    IReadOnlyCollection<Guid> EmployeeIds,
    IReadOnlyCollection<Guid>? ProfileIds,
    int? PerEmployeeCount) : ICommand<Result<DistributionResultDto>>;
