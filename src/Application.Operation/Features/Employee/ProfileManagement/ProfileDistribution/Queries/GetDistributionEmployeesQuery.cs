using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;

public sealed record GetDistributionEmployeesQuery
    : PaginatedRequest, IRequest<Result<PaginatedResult<DistributionEmployeeDto>>>
{
    public Guid UserId { get; init; }
    public string? SearchTerm { get; init; }
    public DistributionEmployeeAvailability? Availability { get; init; }
}
