using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Queries;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Handlers.Queries;

public sealed class GetJobInvitationSummaryDetailsStatsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetJobInvitationSummaryDetailsStatsQuery, IResult<JobInvitationSummaryDetailsStatsDto>>
{
    public async Task<IResult<JobInvitationSummaryDetailsStatsDto>> Handle(
        GetJobInvitationSummaryDetailsStatsQuery query,
        CancellationToken cancellationToken)
    {

        var invitations = unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Where(invitation => invitation.JobId == query.JobId);

        var total = await invitations.CountAsync(cancellationToken);

        var stats = new JobInvitationSummaryDetailsStatsDto
        {
            Total = total,
            New = await invitations.CountAsync(
                invitation => invitation.InvitationStatusId == InvitationStatusIds.NewInvitation,
                cancellationToken),
            Read = await invitations.CountAsync(
                invitation => invitation.InvitationStatusId == InvitationStatusIds.Readed,
                cancellationToken),
            Applied = await invitations.CountAsync(
                invitation => invitation.InvitationStatusId == InvitationStatusIds.Approved,
                cancellationToken),
            Declined = await invitations.CountAsync(
                invitation => invitation.InvitationStatusId == InvitationStatusIds.Rejected,
                cancellationToken),
            Cancelled = await invitations.CountAsync(
                invitation => invitation.InvitationStatusId == InvitationStatusIds.Cancelled,
                cancellationToken),
            Expired = await invitations.CountAsync(
                invitation => invitation.InvitationStatusId == InvitationStatusIds.Closed,
                cancellationToken),
        };

        return Result.Ok(stats);
    }
}
