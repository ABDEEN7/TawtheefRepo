using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Application.Operation.Features.Employee.Dashboard.Queries.Invitations;
using Application.Operation.Features.Employee.Dashboard.Services.Read;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries.Invitations;

internal sealed class GetLatestInvitationsQueryHandler(
    DashboardInvitationsReader reader)
    : IRequestHandler<GetLatestInvitationsQuery, Result<PaginatedResult<LatestInvitationDto>>>
{
    public Task<Result<PaginatedResult<LatestInvitationDto>>> Handle(
        GetLatestInvitationsQuery request,
        CancellationToken ct) =>
        reader.ReadLatestAsync(request, ct);
}
