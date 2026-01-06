using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Religions.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Religions.Queries;

public sealed record GetListReligionsQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<ReligionAdminDto>>>
{
    public string? Search { get; init; }
}
