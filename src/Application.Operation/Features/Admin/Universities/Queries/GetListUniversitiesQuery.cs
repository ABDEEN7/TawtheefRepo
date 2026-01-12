using Application.Operation.Features.Admin.Universities.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Universities.Queries;

public sealed record GetListUniversitiesQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<UniversityAdminDto>>>
{
    public string? Search { get; init; }
    public Guid? CountryId { get; init; }
}
