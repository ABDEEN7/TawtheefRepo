using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Application.Operation.Features.Employee.Dashboard.Queries.Invitations;
using Application.Operation.Features.Employee.Dashboard.Services.Read;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries.Invitations;

internal sealed class GetLatestInvitationsQueryHandler(DashboardInvitationsReader reader)
    : IRequestHandler<GetLatestInvitationsQuery, Result<IReadOnlyList<LatestInvitationDto>>>
{
    public Task<Result<IReadOnlyList<LatestInvitationDto>>> Handle(GetLatestInvitationsQuery request, CancellationToken ct) =>
        reader.ReadLatestAsync(request, ct);
}
