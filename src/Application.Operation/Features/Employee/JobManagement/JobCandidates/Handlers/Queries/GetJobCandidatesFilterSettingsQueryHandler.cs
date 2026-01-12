using Application.Operation.Features.Employee.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobCandidates.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidatesFilterSettingsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetJobCandidatesFilterSettingsQuery, IResult<JobCandidateFilterSettingsDto>>
{
    public async Task<IResult<JobCandidateFilterSettingsDto>> Handle(
        GetJobCandidatesFilterSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<JobCandidateFilterSetting>();

        var dto = await repo.DbSet
            .AsNoTracking()
            .Where(s => s.JobId == request.JobId)
            .Select(s => new JobCandidateFilterSettingsDto
            {
                JobId = s.JobId,
                GenderId = s.GenderId,
                MinimumPoints = s.MinimumPoints,

                CandidateTypePercentages = s.CandidateTypePercentages
                    .Select(p => new JobCandidateTypePercentageDto
                    {
                        CandidateTypeId = p.CandidateTypeId,
                        Percentage = p.Percentage
                    })
                    .ToList(),

                NationalityPercentages = s.NationalityPercentages
                    .Select(p => new JobCandidateNationalityPercentageDto
                    {
                        CandidateTypeId = p.CandidateTypeId,
                        NationalityId = p.NationalityId,
                        Percentage = p.Percentage
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return Result.Ok(dto ?? new JobCandidateFilterSettingsDto { JobId = request.JobId });
    }
}
