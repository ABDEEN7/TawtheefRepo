using Cortex.Mediator.Queries;
using FluentResults;

using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Queries;

public sealed record GetDistributionEmployeesQuery
    : IQuery<Result<IReadOnlyList<DistributionEmployeeDto>>>;
