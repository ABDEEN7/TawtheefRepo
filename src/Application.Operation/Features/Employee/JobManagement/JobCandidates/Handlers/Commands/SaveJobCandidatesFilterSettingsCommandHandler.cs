using Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Commands;

public sealed class SaveJobCandidatesFilterSettingsCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<SaveJobCandidatesFilterSettingsCommand, IResult<JobCandidateFilterSettingsDto>>
{
    public async Task<IResult<JobCandidateFilterSettingsDto>> Handle(
        SaveJobCandidatesFilterSettingsCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Request;
        var typePercentages = dto.CandidateTypePercentages
            .Where(item => item.Percentage > 0)
            .ToList();

        if (typePercentages.Sum(item => item.Percentage) > 100)
            return Result.Fail<JobCandidateFilterSettingsDto>(JobMessages.JobCandidatesFilterPercentagesInvalid);

        if (!AreNationalityBreakdownsValid(typePercentages, dto.NationalityPercentages))
            return Result.Fail<JobCandidateFilterSettingsDto>(JobMessages.JobCandidatesNationalityBreakdownInvalid);

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

    private static bool AreNationalityBreakdownsValid(
        IReadOnlyCollection<JobCandidateTypePercentageDto> typePercentages,
        IReadOnlyCollection<JobCandidateNationalityPercentageDto> nationalityPercentages)
    {
        if (nationalityPercentages.Count == 0)
            return true;

        var typePercentageMap = typePercentages.ToDictionary(item => item.CandidateTypeId, item => item.Percentage);

        foreach (var group in nationalityPercentages.GroupBy(item => item.CandidateTypeId))
        {
            var total = group.Sum(item => item.Percentage);
            if (total <= 0)
                continue;

            if (!typePercentageMap.TryGetValue(group.Key, out var typePercentage))
                return false;

            if (typePercentage <= 0 || total != typePercentage)
                return false;
        }

        return true;
    }
}
