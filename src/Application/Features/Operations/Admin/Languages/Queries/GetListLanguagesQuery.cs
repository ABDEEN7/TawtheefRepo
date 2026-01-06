using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Languages.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Languages.Queries;

public sealed record GetListLanguagesQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<LanguageAdminDto>>>
{
    public string? Search { get; init; }
}
