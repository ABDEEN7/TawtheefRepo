using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Queries;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Handlers.Queries;

public sealed class GetJobInvitationSummaryDetailsStatsQueryHandler(
    IUnitOfWork unitOfWork,
    EmployeeJobAccessContextProvider accessContextProvider)
    : IRequestHandler<GetJobInvitationSummaryDetailsStatsQuery, IResult<JobInvitationSummaryDetailsStatsDto>>
{
    public async Task<IResult<JobInvitationSummaryDetailsStatsDto>> Handle(
        GetJobInvitationSummaryDetailsStatsQuery query,
        CancellationToken cancellationToken)
    {
        var canAccessJob = await unitOfWork.GetEntityRepository<Tawtheef.Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
            .ApplyJobAccessScope(accessContextProvider.GetAccess())
            .AnyAsync(job => job.Id == query.JobId, cancellationToken);
        if (!canAccessJob)
            return Result.Fail<JobInvitationSummaryDetailsStatsDto>(JobMessages.JobNotFound);

        var invitations = unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Where(invitation => invitation.JobId == query.JobId);

        var stats = await invitations
            .GroupBy(_ => 1)
            .Select(group => new JobInvitationSummaryDetailsStatsDto
            {
                Total = group.Count(),
                New = group.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.NewInvitation),
                Read = group.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.Read),
                Applied = group.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.ExamEligible),
                Declined = group.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.Rejected),
                Cancelled = group.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.Cancelled),
                Expired = group.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.Closed),
                PendingAttachmentApproval = group.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.PendingAttachmentApproval),
                ReturnedAttachment = group.Count(invitation =>
                    invitation.InvitationStatusId == InvitationStatusIds.ReturnedAttachment)
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? new JobInvitationSummaryDetailsStatsDto();

        return Result.Ok(stats);
    }
}
