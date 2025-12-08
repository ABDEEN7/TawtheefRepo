using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.ProfileDistribution.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileDistribution.Queries;

public sealed record GetDistributionEmployeesQuery
    : IRequest<Result<IReadOnlyList<DistributionEmployeeDto>>>;
