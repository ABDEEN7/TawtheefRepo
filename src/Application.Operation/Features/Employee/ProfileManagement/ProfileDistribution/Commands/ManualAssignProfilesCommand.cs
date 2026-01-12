using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Commands;

public sealed record ManualAssignProfilesCommand(
    Guid UserId, Guid EmployeeId, IReadOnlyCollection<Guid> ProfileIds)
    : ICommand<Result<DistributionResultDto>>;
