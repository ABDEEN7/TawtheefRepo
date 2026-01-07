using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Handlers.Queries;

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
        var job = await unitOfWork.GetEntityRepository<Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
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
                AdditionalData = job.JobStatus.DisplayOrder
            };

        dto.CurrentBatchNumber = await unitOfWork.GetEntityRepository<Domain.Entities.Recruitment.Invitation>().DbSet
            .AsNoTracking()
            .Where(invitation => invitation.JobId == query.JobId)
            .MaxAsync(invitation => (int?)invitation.BatchNumber, cancellationToken) ?? 0;

        return Result.Ok(dto);
    }
}
