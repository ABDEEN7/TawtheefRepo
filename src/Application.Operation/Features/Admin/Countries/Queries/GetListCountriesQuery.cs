using Application.Operation.Features.Admin.Countries.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Countries.Queries;

public sealed record GetListCountriesQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<CountryAdminDto>>>
{
    public string? Name { get; init; }
    public bool? IsActive { get; init; }
}
