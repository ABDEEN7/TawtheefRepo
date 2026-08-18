using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Dashboard.Queries.Invitations;

public sealed record GetLatestInvitationsQuery : DashboardQueryBase,
    IRequest<Result<PaginatedResult<LatestInvitationDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
