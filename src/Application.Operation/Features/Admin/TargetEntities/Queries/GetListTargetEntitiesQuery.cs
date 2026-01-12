using Application.Operation.Features.Admin.TargetEntities.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.TargetEntities.Queries;

public sealed record GetListTargetEntitiesQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<TargetEntityAdminDto>>>
{
    public string? Search { get; init; }
}
