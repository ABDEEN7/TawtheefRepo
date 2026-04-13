using Application.Operation.Features.Admin.Cities.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Cities.Queries;

public sealed record GetListCitiesQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<CityAdminDto>>>
{
    public Guid? CountryId { get; init; }
    public string? Name { get; init; }
    public bool? IsActive { get; init; }
}
