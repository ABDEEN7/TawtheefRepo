using Application.Operation.Features.Admin.Religions.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Religions.Queries;

public sealed record GetListReligionsQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<ReligionAdminDto>>>
{
    public string? Search { get; init; }
}

