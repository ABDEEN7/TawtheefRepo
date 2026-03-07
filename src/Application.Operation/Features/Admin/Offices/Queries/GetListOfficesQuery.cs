using Application.Operation.Features.Admin.Offices.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Offices.Queries;

public sealed record GetListOfficesQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<OfficeDto>>>
{
    public string? Search { get; init; }
}

