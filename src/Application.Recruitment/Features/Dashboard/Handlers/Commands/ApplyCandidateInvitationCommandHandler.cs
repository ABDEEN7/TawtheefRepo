using Application.Recruitment.Features.Dashboard.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Dashboard.Handlers.Commands;

public sealed class ApplyCandidateInvitationCommandHandler(IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : IRequestHandler<ApplyCandidateInvitationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ApplyCandidateInvitationCommand command,
        CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Include(i => i.Job).ThenInclude(j => j!.JobRequiredAttachments)
            .Include(i => i.Attachments)
            .FirstOrDefaultAsync(
                inv => inv.Id == command.InvitationId && inv.ApplicantId == command.UserId,
                cancellationToken);

        if (invitation is null)
            return Result.Fail<Unit>(ErrorsCodes.InvitationNotFound);

        var mandatoryAttachments = invitation.Job?.JobRequiredAttachments.Where(a => a.IsMandatory) ?? [];
        foreach (var mandatory in mandatoryAttachments)
        {
            var hasAttachment =
                invitation.Attachments.Any(a => a.JobRequiredAttachmentId == mandatory.Id && !a.IsReturned);
            if (!hasAttachment)
                return Result.Fail<Unit>(ErrorsCodes.JobRequiredAttachmentMissing);
        }

        var canSubmit = invitation.InvitationStatusId == InvitationStatusIds.NewInvitation
                        || invitation.InvitationStatusId == InvitationStatusIds.Read
                        || invitation.InvitationStatusId == InvitationStatusIds.PendingAttachmentApproval
                        || invitation.InvitationStatusId == InvitationStatusIds.ReturnedAttachment
                        || invitation.InvitationStatusId == InvitationStatusIds.ExamEligible;

        if (!canSubmit)
            return Result.Fail<Unit>(ErrorsCodes.InvitationStatusChangeNotAllowed);

        bool needsApproval = invitation.Attachments.Any();
        var newStatus = needsApproval
            ? InvitationStatusIds.PendingAttachmentApproval
            : InvitationStatusIds.ExamEligible;

        var acceptedAt = timeProvider.GetUtcNow().UtcDateTime;
        var isInitialAcceptance = invitation.InvitationStatusId == InvitationStatusIds.NewInvitation
                                  || invitation.InvitationStatusId == InvitationStatusIds.Read;

        return await unitOfWork.ExecuteInTransactionAsync<Result<Unit>>(
            async ct =>
            {
                var exceptionRepository =
                    unitOfWork.GetEntityRepository<InvitationException>();

                await exceptionRepository.DbSet
                    .Where(invitationException =>
                        invitationException.InvitationId == command.InvitationId &&
                        invitationException.Status == InvitationExceptionStatus.InvitationSent)
                    .ExecuteUpdateAsync(
                        setters => setters
                            .SetProperty(
                                invitationException => invitationException.Status,
                                InvitationExceptionStatus.Applied)
                            .SetProperty(
                                invitationException => invitationException.UpdatedDate,
                                acceptedAt),
                        ct);

                var hasConflictingException = await exceptionRepository.DbSet
                    .AsNoTracking()
                    .AnyAsync(
                        invitationException =>
                            invitationException.InvitationId == command.InvitationId &&
                            invitationException.Status != InvitationExceptionStatus.Applied,
                        ct);

                if (hasConflictingException)
                {
                    return Result.Fail<Unit>(
                        ErrorsCodes.InvitationStatusChangeNotAllowed);
                }

                var invitationUpdate = unitOfWork
                    .GetEntityRepository<Invitation>()
                    .DbSet
                    .Where(current =>
                        current.Id == command.InvitationId &&
                        current.ApplicantId == command.UserId);

                invitationUpdate = isInitialAcceptance
                    ? invitationUpdate.Where(current =>
                        !current.IsAccepted &&
                        (
                            current.InvitationStatusId == InvitationStatusIds.NewInvitation ||
                            current.InvitationStatusId == InvitationStatusIds.Read
                        ))
                    : invitationUpdate.Where(current =>
                        current.InvitationStatusId == invitation.InvitationStatusId);

                var updated = await invitationUpdate.ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            current => current.InvitationStatusId,
                            newStatus)
                        .SetProperty(
                            current => current.IsAccepted,
                            true)
                        .SetProperty(
                            current => current.AcceptedAt,
                            acceptedAt)
                        .SetProperty(
                            current => current.UpdatedDate,
                            acceptedAt)
                        .SetProperty(
                            current => current.UpdatedById,
                            command.UserId),
                    ct);

                if (updated == 0)
                {
                    var stillExists = await unitOfWork
                        .GetEntityRepository<Invitation>()
                        .DbSet
                        .AsNoTracking()
                        .AnyAsync(
                            current =>
                                current.Id == command.InvitationId &&
                                current.ApplicantId == command.UserId,
                            ct);

                    return Result.Fail<Unit>(
                        stillExists
                            ? ErrorsCodes.InvitationStatusChangeNotAllowed
                            : ErrorsCodes.InvitationNotFound);
                }

                return Result.Ok(Unit.Value);
            },
            cancellationToken);
    }
}
