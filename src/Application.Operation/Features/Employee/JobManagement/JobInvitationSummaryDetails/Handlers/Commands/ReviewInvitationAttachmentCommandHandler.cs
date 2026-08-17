using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Application.Operation.Features.Employee.Common.Access;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Events.Operation;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Handlers.Commands;

public class ReviewInvitationAttachmentCommandHandler(
    IUnitOfWork unitOfWork,
    EmployeeJobAccessContextProvider accessContextProvider)
    : IRequestHandler<ReviewInvitationAttachmentCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ReviewInvitationAttachmentCommand request, CancellationToken cancellationToken)
    {
        var accessibleJobIds = unitOfWork.GetEntityRepository<Tawtheef.Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
            .ApplyJobAccessScope(accessContextProvider.GetAccess())
            .Select(job => job.Id);
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .Include(x => x.Attachments).ThenInclude(x => x.JobRequiredAttachment)
            .FirstOrDefaultAsync(x =>
                x.Id == request.InvitationId && accessibleJobIds.Contains(x.JobId), cancellationToken);

        if (invitation == null)
            return Result.Fail(ErrorsCodes.InvitationNotFound);

        var attachment = invitation.Attachments.FirstOrDefault(x => x.Id == request.AttachmentId);
        if (attachment == null)
            return Result.Fail(ErrorsCodes.AttachmentNotFound);

        var oldIsApproved = attachment.IsApproved;
        var oldIsReturned = attachment.IsReturned;

        // Update attachment
        attachment.IsApproved = request.IsApproved;
        attachment.IsReturned = !request.IsApproved && !string.IsNullOrWhiteSpace(request.ReviewNote);
        attachment.ReviewNote = request.IsApproved ? null : request.ReviewNote;

        // Log audit
        var auditRepo = unitOfWork.GetEntityRepository<ActionLog>();
        var actionType = request.IsApproved ? "AttachmentApproved" : "AttachmentReturned";
        var note = JsonSerializer.Serialize(new
        {
            eventType = "InvitationAttachmentReview",
            actionKind = "Update",
            actionType,
            section = "JobInvitationAttachment",
            invitationId = request.InvitationId,
            attachmentId = request.AttachmentId,
            attachmentTitle = attachment.AttachmentTitleEn,
            status = request.IsApproved ? "Approved" : "Returned",
            reviewNote = request.IsApproved ? null : request.ReviewNote,
            changedFields = new[]
            {
                new { field = "IsApproved", oldValue = oldIsApproved.ToString(), newValue = attachment.IsApproved.ToString() },
                new { field = "IsReturned", oldValue = oldIsReturned.ToString(), newValue = attachment.IsReturned.ToString() }
            },
            message = request.IsApproved ? "Attachment approved" : "Attachment returned"
        });
        
        await auditRepo.AddAsync(new ActionLog
        {
            UserId = invitation.ApplicantId,
            LogType = ActionLogType.Employee,
            ActionType = actionType,
            Notes = note,
            Section = "JobInvitationAttachment",
        }, cancellationToken);

        // If returned, dispatch event
        if (attachment.IsReturned)
        {
            var evt = new InvitationAttachmentReturnedEvent(
                invitation.ApplicantId,
                invitation.Id,
                attachment.AttachmentTitleEn,
                request.ReviewNote
            );
            attachment.AddDomainEvent(evt);
            
            // Set overall invitation status to ReturnedAttachment if any file is returned
            invitation.ChangeInvitationStatus(InvitationStatusIds.ReturnedAttachment);
        }

        // If all approved, move to Submitted
        if (invitation.Attachments.All(x => x.IsApproved))
        {
            invitation.ChangeInvitationStatus(InvitationStatusIds.ExamEligible);
        }
        else if (!invitation.Attachments.Any(x => x.IsReturned) 
                 && invitation.Attachments.Any(x => !x.IsApproved))
        {
            // If none are returned but some are still pending approval
            invitation.ChangeInvitationStatus(InvitationStatusIds.PendingAttachmentApproval);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
