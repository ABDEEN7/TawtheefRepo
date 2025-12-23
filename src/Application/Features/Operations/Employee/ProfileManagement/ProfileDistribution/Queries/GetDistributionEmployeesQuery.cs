using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Queries;

public sealed record GetDistributionEmployeesQuery
    : IRequest<Result<IReadOnlyList<DistributionEmployeeDto>>>;
