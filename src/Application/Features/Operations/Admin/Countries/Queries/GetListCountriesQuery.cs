using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Countries.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Countries.Queries;

public sealed record GetListCountriesQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<CountryAdminDto>>>
{
    public string? Name { get; init; }
    public bool? IsActive { get; init; }
}
