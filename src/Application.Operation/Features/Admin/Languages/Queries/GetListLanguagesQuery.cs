using Application.Operation.Features.Admin.Languages.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Languages.Queries;

public sealed record GetListLanguagesQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<LanguageAdminDto>>>
{
    public string? Search { get; init; }
}
