using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidatesFilterSettingsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetJobCandidatesFilterSettingsQuery, IResult<JobCandidateFilterSettingsDto>>
{
    public async Task<IResult<JobCandidateFilterSettingsDto>> Handle(
        GetJobCandidatesFilterSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var settings = await unitOfWork.GetEntityRepository<JobCandidateFilterSetting>().DbSet
            .AsNoTracking()
            .Include(setting => setting.CandidateTypePercentages)
            .Include(setting => setting.NationalityPercentages)
            .FirstOrDefaultAsync(setting => setting.JobId == request.JobId, cancellationToken);

        if (settings is null)
        {
            return Result.Ok(new JobCandidateFilterSettingsDto { JobId = request.JobId });
        }

        var result = new JobCandidateFilterSettingsDto
        {
            JobId = settings.JobId,
            GenderId = settings.GenderId,
            MinimumPoints = settings.MinimumPoints,
            CandidateTypePercentages = settings.CandidateTypePercentages
                .Select(item => new JobCandidateTypePercentageDto
                {
                    CandidateTypeId = item.CandidateTypeId,
                    Percentage = item.Percentage
                })
                .ToList(),
            NationalityPercentages = settings.NationalityPercentages
                .Select(item => new JobCandidateNationalityPercentageDto
                {
                    CandidateTypeId = item.CandidateTypeId,
                    NationalityId = item.NationalityId,
                    Percentage = item.Percentage
                })
                .ToList()
        };

        return Result.Ok(result);
    }
}
