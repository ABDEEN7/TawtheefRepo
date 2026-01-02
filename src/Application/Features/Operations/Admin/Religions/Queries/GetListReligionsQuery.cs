using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Religions.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Religions.Queries;

public sealed record GetListReligionsQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<ReligionAdminDto>>>
{
    public string? Search { get; init; }
}
