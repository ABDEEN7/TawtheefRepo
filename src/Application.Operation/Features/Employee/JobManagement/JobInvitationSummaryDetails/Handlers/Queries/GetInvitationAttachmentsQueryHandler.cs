using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Handlers.Queries;

public class GetInvitationAttachmentsQueryHandler(
    IUnitOfWork unitOfWork,
    IMediaUrlResolver mediaService)
    : IRequestHandler<GetInvitationAttachmentsQuery, Result<List<InvitationAttachmentDto>>>
{
    public async Task<Result<List<InvitationAttachmentDto>>> Handle(GetInvitationAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .Include(i => i.Job).ThenInclude(j => j!.JobRequiredAttachments)
            .Include(i => i.Attachments)
            .FirstOrDefaultAsync(i => i.Id == request.InvitationId, cancellationToken);

        if (invitation == null)
            return Result.Fail(ErrorsCodes.InvitationNotFound);

        var requiredAttachments = invitation.Job?.JobRequiredAttachments ?? new List<Tawtheef.Domain.Entities.Recruitment.JobDetails.JobRequiredAttachment>();
        
        var dtos = new List<InvitationAttachmentDto>();

        var resourceIds = invitation.Attachments?.Select(a => a.ResourceId).ToList() ?? new List<Guid>();
        var resources = await unitOfWork.GetEntityRepository<Resource>().DbSet
            .Where(r => resourceIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, cancellationToken);

        foreach (var req in requiredAttachments)
        {
            var att = invitation.Attachments?.FirstOrDefault(a => a.JobRequiredAttachmentId == req.Id);
            var dto = new InvitationAttachmentDto
            {
                JobRequiredAttachmentId = req.Id,
                TitleEn = req.TitleEn,
                TitleAr = req.TitleAr,
                IsMandatory = req.IsMandatory
            };

            if (att != null)
            {
                dto.Id = att.Id;
                dto.IsApproved = att.IsApproved;
                dto.IsReturned = att.IsReturned;
                dto.ReviewNote = att.ReviewNote;
                dto.ResourceId = att.ResourceId;
                
                if (resources.TryGetValue(att.ResourceId, out var resource))
                {
                    dto.ResourceUrl = mediaService.ResolveAbsolute(resource.Url);
                    dto.ResourceName = resource.Name;
                }
            }

            dtos.Add(dto);
        }

        return Result.Ok(dtos);
    }
}
