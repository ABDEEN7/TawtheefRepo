using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Commands;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Events.Operation.Employee.JobCandidates;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Commands;

public sealed class SendJobCandidateInvitationsCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<SendJobCandidateInvitationsCommand, IResult<SendJobCandidateInvitationsResult>>
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

        var sentEmailCount = candidates.Count(candidate =>
            !string.IsNullOrWhiteSpace(candidate.Applicant?.Email));
        var sentSmsCount = candidates.Count(candidate =>
            !string.IsNullOrWhiteSpace(candidate.Applicant?.PhoneNumber));

        var candidatesByInvitationId = candidates.ToDictionary(candidate => candidate.InvitationId);

        var invitationIds = candidates.Select(candidate => candidate.InvitationId).Distinct().ToList();
        var invitations = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .Where(invitation => invitationIds.Contains(invitation.Id))
            .ToListAsync(cancellationToken);

        foreach (var invitation in invitations)
        {
            if (!candidatesByInvitationId.TryGetValue(invitation.Id, out var candidate))
            {
                continue;
            }

            var jobTitle = candidate.Job?.TitleEn ?? candidate.Job?.TitleAr ?? string.Empty;
            var domainEvent = new JobCandidateInvitationSentDomainEvent(
                invitation.Id,
                candidate.ApplicantId,
                candidate.Applicant?.Email,
                candidate.Applicant?.PhoneNumber,
                jobTitle,
                DateTimeOffset.UtcNow);

            invitation.InvitationStatusId = InvitationStatusIds.NewInvitation;
            invitation.AddDomainEvent(domainEvent);
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
