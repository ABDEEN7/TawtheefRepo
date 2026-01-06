using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.TargetEntities.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.TargetEntities.Queries;

public sealed record GetListTargetEntitiesQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<TargetEntityAdminDto>>>
{
    public string? Search { get; init; }
}
