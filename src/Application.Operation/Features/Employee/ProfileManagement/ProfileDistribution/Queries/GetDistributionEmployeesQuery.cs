using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;

public sealed record GetDistributionEmployeesQuery(Guid UserId = default)
    : IRequest<Result<IReadOnlyList<DistributionEmployeeDto>>>;

