using Application.Recruitment.Features.JobDetails.DTOs;
using Application.Recruitment.Features.JobDetails.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.JobDetails.Handlers.Queries;

public sealed class GetCandidateJobDetailsQueryHandler(
    IUnitOfWork unitOfWork,
    IJobRepository jobRepository,
    IMediaUrlResolver mediaService,
    IMapper mapper)
    : IRequestHandler<GetCandidateJobDetailsQuery, IResult<CandidateJobDetailsDto>>
{
    public async Task<IResult<CandidateJobDetailsDto>> Handle(GetCandidateJobDetailsQuery query, CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .Include(x => x.InvitationStatus)
            .Include(x => x.Attachments)
            .AsNoTracking()
            .FirstOrDefaultAsync(inv => inv.Id == query.InvitationId && inv.ApplicantId == query.UserId, cancellationToken);

        if (invitation is null)
            return Result.Fail<CandidateJobDetailsDto>(ErrorsCodes.InvitationNotFound);

        var jobResult = await jobRepository.GetByIdWithDetailsAsync(invitation.JobId, cancellationToken);

        if (jobResult.IsFailed)
            return Result.Fail<CandidateJobDetailsDto>(jobResult.Errors);

        var job = jobResult.Value;

        if (job is null)
            return Result.Fail<CandidateJobDetailsDto>(JobMessages.JobNotFound);

        var jobDto = mapper.Map<CandidateJobDetailsDto>(job);
        
        // Map invitation status
        jobDto.InvitationStatusId = invitation.InvitationStatusId;
        if (invitation.InvitationStatus != null)
        {
            jobDto.InvitationStatus = mapper.Map<DropdownOptions>(invitation.InvitationStatus);
        }

        // Map already uploaded attachments & resources
        if (jobDto.RequiredAttachments != null)
        {
            var resourceIds = invitation.Attachments.Select(a => a.ResourceId).ToList();
            var resources = await unitOfWork.GetEntityRepository<Resource>().DbSet
                .AsNoTracking()
                .Where(r => resourceIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, cancellationToken);

            foreach (var req in jobDto.RequiredAttachments)
            {
                var att = invitation.Attachments.FirstOrDefault(a => a.JobRequiredAttachmentId == req.Id);
                if (att != null)
                {
                    req.AttachmentId = att.Id;
                    req.ResourceId = att.ResourceId;
                    req.IsApproved = att.IsApproved;
                    req.IsReturned = att.IsReturned;
                    req.ReviewNote = att.ReviewNote;
                    
                    if (resources.TryGetValue(att.ResourceId, out var resource))
                    {
                        req.ResourceUrl = mediaService.ResolveAbsolute(resource.Url);
                        req.ResourceName = resource.Name;
                    }
                }
            }
        }

        return Result.Ok(jobDto);
    }
}

