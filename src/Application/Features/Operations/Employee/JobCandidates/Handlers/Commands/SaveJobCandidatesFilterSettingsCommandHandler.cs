using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Commands;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Commands;

public sealed class SaveJobCandidatesFilterSettingsCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<SaveJobCandidatesFilterSettingsCommand, IResult<JobCandidateFilterSettingsDto>>
{
    public async Task<IResult<JobCandidateFilterSettingsDto>> Handle(
        SaveJobCandidatesFilterSettingsCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Request;
        var repo = unitOfWork.GetEntityRepository<JobCandidateFilterSetting>();

        var settings = await repo.DbSet
            .Include(setting => setting.CandidateTypePercentages)
            .Include(setting => setting.NationalityPercentages)
            .FirstOrDefaultAsync(setting => setting.JobId == dto.JobId, cancellationToken);

        if (settings is null)
        {
            settings = new JobCandidateFilterSetting
            {
                JobId = dto.JobId
            };
            await repo.AddAsync(settings);
        }

        settings.GenderId = dto.GenderId;
        settings.MinimumPoints = dto.MinimumPoints;

        if (settings.CandidateTypePercentages.Count > 0)
        {
            unitOfWork.RemoveRange(settings.CandidateTypePercentages.ToList());
            settings.CandidateTypePercentages.Clear();
        }

        if (settings.NationalityPercentages.Count > 0)
        {
            unitOfWork.RemoveRange(settings.NationalityPercentages.ToList());
            settings.NationalityPercentages.Clear();
        }

        settings.CandidateTypePercentages = dto.CandidateTypePercentages
            .Select(item => new JobCandidateTypePercentage
            {
                CandidateTypeId = item.CandidateTypeId,
                Percentage = item.Percentage
            })
            .ToList();

        settings.NationalityPercentages = dto.NationalityPercentages
            .Select(item => new JobCandidateNationalityPercentage
            {
                CandidateTypeId = item.CandidateTypeId,
                NationalityId = item.NationalityId,
                Percentage = item.Percentage
            })
            .ToList();

        await unitOfWork.SaveChangesAsync(cancellationToken);

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
