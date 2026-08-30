using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;

public sealed record GetDistributionEmployeeLookupQuery(Guid UserId)
    : IRequest<Result<IReadOnlyList<DistributionEmployeeLookupDto>>>;
