using Application.Operation.Features.Employee.Locations.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Locations.Queries;

public sealed record ListLocationsQuery
    : PaginatedRequest, IRequest<IResult<PaginatedResult<LocationDto>>>
{
    public string? Search { get; init; }
}
