using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Commands;

public sealed record ManualAssignProfilesCommand(
    Guid UserId, Guid EmployeeId, IReadOnlyCollection<Guid> ProfileIds)
    : ICommand<Result<DistributionResultDto>>;
