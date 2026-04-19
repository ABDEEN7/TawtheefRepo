using Application.Recruitment.Features.Dashboard.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Dashboard.Handlers.Commands;

public class DeleteInvitationAttachmentCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<DeleteInvitationAttachmentCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteInvitationAttachmentCommand request, CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == request.InvitationId && x.ApplicantId == request.UserId, cancellationToken);

        if (invitation == null)
            return Result.Fail(ErrorsCodes.InvitationNotFound);

        if (!invitation.CanModifyAttachments)
            return Result.Fail(ErrorsCodes.InvitationStatusChangeNotAllowed);

        var attachment = invitation.Attachments.FirstOrDefault(x => x.Id == request.AttachmentId);

        if (attachment == null)
            return Result.Fail(ErrorsCodes.AttachmentNotFound);

        if (attachment.IsApproved)
            return Result.Fail(ErrorsCodes.CanNotModifiedApprovedDocument);

        invitation.Attachments.Remove(attachment);
        await unitOfWork.GetEntityRepository<InvitationAttachment>().DeleteAsync(attachment);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
