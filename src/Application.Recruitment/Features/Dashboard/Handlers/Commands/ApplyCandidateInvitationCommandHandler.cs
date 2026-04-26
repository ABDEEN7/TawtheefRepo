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
    public async Task<IResult<Unit>> Handle(ApplyCandidateInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
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
            var hasAttachment = invitation.Attachments.Any(a => a.JobRequiredAttachmentId == mandatory.Id && !a.IsReturned);
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

        invitation.ChangeInvitationStatus(newStatus);
        invitation.IsAccepted = true;
        invitation.AcceptedAt = timeProvider.GetUtcNow().UtcDateTime;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}

