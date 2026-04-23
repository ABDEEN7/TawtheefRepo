using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobSpecialization = Application.Operation.Features.Employee.JobManagement.JobCandidates.Models.JobSpecialization;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidatesFilterSettingsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobCandidatesFilterSettingsQuery, IResult<JobCandidateFilterSettingsDto>>
{
    public async Task<IResult<JobCandidateFilterSettingsDto>> Handle(
        GetJobCandidatesFilterSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<JobCandidateFilterSetting>();
        var isFilterSettingsExist = await repo.DbSet.AnyAsync(s => s.JobId == request.JobId, cancellationToken);

        if (!isFilterSettingsExist)
        {
            // create default settings with main (main-major and sub-major) job specializations
            var job = await unitOfWork.GetEntityRepository<Job>()
                .DbSet
                .AsNoTracking()
                .AsSplitQuery()
                .Where(j => j.Id == request.JobId)
                .Select(j => new { j.Id, j.MajorId, j.SubMajorId })
                .FirstOrDefaultAsync(cancellationToken);
            if (job is null)
                return Result.Fail<JobCandidateFilterSettingsDto>(JobMessages.JobNotFound);

            if (job.MajorId is not null && job.SubMajorId is not null)
            {
                await repo.AddAsync(
                    new JobCandidateFilterSetting
                    {
                        JobId = request.JobId,
                        SelectedSpecializations = [new JobCandidateFilterSpecialization
                        {
                            MajorId = job.MajorId!.Value, 
                            SubMajorId = job.SubMajorId!.Value
                        }]
                    }, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
        
        var dto = await repo.DbSet
            .AsNoTracking()
            .AsSplitQuery()
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
                    .ToList(),

                SelectedSpecializations = s.SelectedSpecializations
                    .Select(p => new JobSpecialization(p.MajorId, p.SubMajorId))
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return Result.Ok(dto ?? new JobCandidateFilterSettingsDto { JobId = request.JobId });
    }
}

