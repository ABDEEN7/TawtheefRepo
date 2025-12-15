using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Queries;

public sealed record GetListOfficesQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<OfficeDto>>>
{
    public string? Search { get; init; }
}
