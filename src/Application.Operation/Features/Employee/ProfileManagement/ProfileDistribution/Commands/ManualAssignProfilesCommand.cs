using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Commands;

public sealed record ManualAssignProfilesCommand(
    Guid UserId, Guid EmployeeId, IReadOnlyCollection<Guid> ProfileIds)
    : IRequest<Result<DistributionResultDto>>;

