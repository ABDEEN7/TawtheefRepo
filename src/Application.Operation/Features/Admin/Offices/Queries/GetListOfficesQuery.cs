using Application.Operation.Features.Admin.Offices.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Offices.Queries;

public sealed record GetListOfficesQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<OfficeDto>>>
{
    public string? Search { get; init; }
}
