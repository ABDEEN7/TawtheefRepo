using Application.Operation.Features.Admin.TargetEntities.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.TargetEntities.Queries;

public sealed record GetListTargetEntitiesQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<TargetEntityAdminDto>>>
{
    public string? Search { get; init; }
}

