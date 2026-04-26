using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Queries;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Handlers.Queries;

public sealed class GetJobInvitationSummaryDetailsStatsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobInvitationSummaryDetailsStatsQuery, IResult<JobInvitationSummaryDetailsStatsDto>>
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
                invitation => invitation.InvitationStatusId == InvitationStatusIds.Read,
                cancellationToken),
            Applied = await invitations.CountAsync(
                invitation => invitation.InvitationStatusId == InvitationStatusIds.ExamEligible,
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
            PendingAttachmentApproval = await invitations.CountAsync(
                invitation => invitation.InvitationStatusId == InvitationStatusIds.PendingAttachmentApproval,
                cancellationToken),
            ReturnedAttachment = await invitations.CountAsync(
                invitation => invitation.InvitationStatusId == InvitationStatusIds.ReturnedAttachment,
                cancellationToken),
        };

        return Result.Ok(stats);
    }
}

