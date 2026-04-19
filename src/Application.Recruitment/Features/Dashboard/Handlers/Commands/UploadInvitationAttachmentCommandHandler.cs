using Application.Recruitment.Features.Dashboard.Commands;
using Application.Recruitment.Features.Dashboard.DTOs;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Dashboard.Handlers.Commands;

public class UploadInvitationAttachmentCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) 
    : IRequestHandler<UploadInvitationAttachmentCommand, Result<InvitationAttachmentDto>>
{
    public async Task<Result<InvitationAttachmentDto>> Handle(UploadInvitationAttachmentCommand request, CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .Include(x => x.Job).ThenInclude(j => j!.JobRequiredAttachments)
            .FirstOrDefaultAsync(x => x.Id == request.InvitationId && x.ApplicantId == request.UserId, cancellationToken);

        if (invitation == null)
            return Result.Fail(ErrorsCodes.InvitationNotFound);
            
        var requiredAttachment = invitation.Job?.JobRequiredAttachments.FirstOrDefault(x => x.Id == request.JobRequiredAttachmentId);
        if (requiredAttachment == null)
             return Result.Fail(ErrorsCodes.AttachmentNotFound);

        if (!invitation.CanModifyAttachments)
            return Result.Fail(ErrorsCodes.InvitationStatusChangeNotAllowed);

        var existingAttachment = await unitOfWork
            .GetEntityRepository<InvitationAttachment>()
            .DbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x =>
                    x.InvitationId == request.InvitationId &&
                    x.JobRequiredAttachmentId == request.JobRequiredAttachmentId,
                cancellationToken);
        
        if (existingAttachment is { IsApproved: true })
            return Result.Fail(ErrorsCodes.CanNotModifiedApprovedDocument);

        var uploadPath = await InvitationAttachmentUploadPathFactory.CreateAsync(
            request.InvitationId, request.File, false, cancellationToken);
            
        var uploadResult = await mediator.Send(
            new UploadAttachmentCommand(request.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, request.File),
            cancellationToken);

        if (uploadResult.IsFailed)
            return Result.Fail(ErrorsCodes.JobRequiredAttachmentUploadFailed);

        var resourceId = uploadResult.Value.ResourceId;
        if (resourceId == Guid.Empty)
            return Result.Fail(ErrorsCodes.JobRequiredAttachmentUploadFailed);

        var resourceRepository = unitOfWork.GetEntityRepository<Resource>();
        var resource = await resourceRepository.DbSet.FirstOrDefaultAsync(r => r.Id == resourceId, cancellationToken);
        if (resource is null)
             return Result.Fail(ErrorsCodes.JobRequiredAttachmentUploadFailed);

        if (existingAttachment is not null)
        {
            if (existingAttachment.IsDeleted)
            {
                // "restore" instead of insert
                existingAttachment.IsDeleted = false;
                existingAttachment.DeletedById = null;
                existingAttachment.DeletedDate = null;
            }
            existingAttachment.ResourceId = resourceId;
            existingAttachment.IsReturned = false;
            existingAttachment.ReviewNote = null;
            existingAttachment.AttachmentTitleEn = requiredAttachment.TitleEn;
            existingAttachment.AttachmentTitleAr = requiredAttachment.TitleAr;
            existingAttachment.Resource = resource;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new InvitationAttachmentDto
            {
                Id = existingAttachment.Id,
                ResourceId = existingAttachment.ResourceId,
                AttachmentTitleEn = existingAttachment.AttachmentTitleEn,
                AttachmentTitleAr = existingAttachment.AttachmentTitleAr
            });
        }

        var attachment = new InvitationAttachment
        {
            InvitationId = request.InvitationId,
            JobRequiredAttachmentId = request.JobRequiredAttachmentId,
            ResourceId = resourceId,
            Resource = resource,
            AttachmentTitleEn = requiredAttachment.TitleEn,
            AttachmentTitleAr = requiredAttachment.TitleAr,
            IsApproved = false,
            IsReturned = false
        };

        invitation.Attachments.Add(attachment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new InvitationAttachmentDto
        {
            Id = attachment.Id,
            ResourceId = attachment.ResourceId,
            AttachmentTitleEn = attachment.AttachmentTitleEn,
            AttachmentTitleAr = attachment.AttachmentTitleAr
        });
    }
}
