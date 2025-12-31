using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Queries;

public sealed record GetListUniversitiesQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<UniversityAdminDto>>>
{
    public string? Search { get; init; }
    public Guid? CountryId { get; init; }
}
