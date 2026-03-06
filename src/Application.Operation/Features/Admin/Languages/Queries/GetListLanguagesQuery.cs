using Application.Operation.Features.Admin.Languages.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Languages.Queries;

public sealed record GetListLanguagesQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<LanguageAdminDto>>>
{
    public string? Search { get; init; }
}

