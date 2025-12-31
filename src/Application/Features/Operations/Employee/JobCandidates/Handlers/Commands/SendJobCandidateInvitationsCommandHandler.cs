using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Commands;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Commands;

public sealed class SendJobCandidateInvitationsCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    ISmsSender smsSender)
    : IRequestHandler<SendJobCandidateInvitationsCommand, IResult<SendJobCandidateInvitationsResult>>
{
    public async Task<IResult<SendJobCandidateInvitationsResult>> Handle(
        SendJobCandidateInvitationsCommand request,
        CancellationToken cancellationToken)
    {
        var query = JobCandidatesQueryBuilder.Build(unitOfWork, request.JobId, request.Filter);

        if (request.InvitationIds is { Count: > 0 })
        {
            query = query.Where(candidate => request.InvitationIds.Contains(candidate.InvitationId));
        }

        var candidates = await query.ToListAsync(cancellationToken);
        if (candidates.Count == 0)
        {
            return Result.Ok(new SendJobCandidateInvitationsResult
            {
                TotalTargets = 0,
                SentEmailCount = 0,
                SentSmsCount = 0,
                UpdatedStatusCount = 0
            });
        }

        var sentEmailCount = 0;
        var sentSmsCount = 0;

        foreach (var candidate in candidates)
        {
            var jobTitle = candidate.Job?.TitleEn ?? candidate.Job?.TitleAr ?? "";
            var body = string.IsNullOrWhiteSpace(jobTitle)
                ? "You have been invited to apply for a job on Tawtheef."
                : $"You have been invited to apply for {jobTitle} on Tawtheef.";

            if (!string.IsNullOrWhiteSpace(candidate.Applicant?.Email))
            {
                var emailResult = await emailSender.SendAsync(
                    candidate.Applicant.Email,
                    "Job Invitation",
                    body,
                    cancellationToken);

                if (emailResult.ok)
                {
                    sentEmailCount++;
                }
            }

            if (!string.IsNullOrWhiteSpace(candidate.Applicant?.PhoneNumber))
            {
                var smsResult = await smsSender.SendAsync(
                    candidate.Applicant.PhoneNumber,
                    body,
                    cancellationToken);

                if (smsResult.ok)
                {
                    sentSmsCount++;
                }
            }
        }

        var invitationIds = candidates.Select(candidate => candidate.InvitationId).Distinct().ToList();
        var invitations = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .Where(invitation => invitationIds.Contains(invitation.Id))
            .ToListAsync(cancellationToken);

        foreach (var invitation in invitations)
        {
            invitation.InvitationStatusId = InvitationStatusIds.NewInvitation;
        }

        var updatedCount = invitations.Count > 0
            ? await unitOfWork.SaveChangesAsync(cancellationToken)
            : 0;

        return Result.Ok(new SendJobCandidateInvitationsResult
        {
            TotalTargets = candidates.Count,
            SentEmailCount = sentEmailCount,
            SentSmsCount = sentSmsCount,
            UpdatedStatusCount = updatedCount
        });
    }
}
