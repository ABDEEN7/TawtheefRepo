using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;

public sealed record GetDistributionEmployeesQuery
    : IQuery<Result<IReadOnlyList<DistributionEmployeeDto>>>;
