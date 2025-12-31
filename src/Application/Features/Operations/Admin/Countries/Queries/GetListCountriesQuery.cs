using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Countries.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Countries.Queries;

public sealed record GetListCountriesQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<CountryAdminDto>>>
{
    public string? Name { get; init; }
    public bool? IsActive { get; init; }
}
