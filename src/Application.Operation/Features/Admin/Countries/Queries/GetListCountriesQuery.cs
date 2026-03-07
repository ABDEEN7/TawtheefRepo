using Application.Operation.Features.Admin.Countries.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Countries.Queries;

public sealed record GetListCountriesQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<CountryAdminDto>>>
{
    public string? Name { get; init; }
    public bool? IsActive { get; init; }
}

