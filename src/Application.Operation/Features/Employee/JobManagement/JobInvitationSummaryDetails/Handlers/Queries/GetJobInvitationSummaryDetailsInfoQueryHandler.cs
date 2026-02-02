using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Handlers.Queries;

public sealed class GetJobInvitationSummaryDetailsInfoQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILocalizationService localizationService)
    : IQueryHandler<GetJobInvitationSummaryDetailsInfoQuery, IResult<JobInvitationSummaryDetailsInfoDto>>
{
    public async Task<IResult<JobInvitationSummaryDetailsInfoDto>> Handle(
        GetJobInvitationSummaryDetailsInfoQuery query,
        CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<Tawtheef.Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
            .Include(job => job.Department)
            .Include(job => job.JobStatus)  
            .FirstOrDefaultAsync(j => j.Id == query.JobId, cancellationToken);

        if (job is null)
            return Result.Fail<JobInvitationSummaryDetailsInfoDto>(JobMessages.JobNotFound);

        var dto = mapper.Map<JobInvitationSummaryDetailsInfoDto>(job);
        dto.JobName = localizationService.GetLocalizedValue(job.TitleAr, job.TitleEn);
        dto.DepartmentName = localizationService.GetLocalizedName(job.Department);
        dto.JobStatus = job.JobStatus == null
            ? new DropdownOptions()
            : new DropdownOptions
            {
                Id = job.JobStatus.Id,
                BackendName = job.JobStatus.BackendName,
                Name = localizationService.GetLocalizedName(job.JobStatus),
                Description = localizationService.GetLocalizedDescription(job.JobStatus),
                AdditionalData = new
                {
                    nameAr = job.JobStatus.NameAr,
                    nameEn = job.JobStatus.NameEn,
                    displayOrder = job.JobStatus.DisplayOrder
                }
            };

        dto.CurrentBatchNumber = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Where(invitation => invitation.JobId == query.JobId)
            .OrderByDescending(invitation => invitation.CreatedDate)
            .Select(invitation => (Guid?)invitation.BatchNumber)
            .FirstOrDefaultAsync(cancellationToken);
        return Result.Ok(dto);
    }
}
