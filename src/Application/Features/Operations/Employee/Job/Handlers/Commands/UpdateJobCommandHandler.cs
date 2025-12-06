using FluentResults;
using Mapster;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class UpdateJobCommandHandler(
    IJobRepository jobRepository,
    IJobSkillRepository jobSkillRepository,
    IJobConditionRepository jobConditionRepository,
    IJobDegreeRepository jobDegreeRepository,
    IResidentsBreakdownRepository residentBreakdownRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        var existingJobResult = await jobRepository.GetByIdWithDetailsAsync(request.JobId);
        if (existingJobResult.IsFailed || existingJobResult.Value == null)
            return Result.Fail<Unit>(JobValidationMessages.JOB_NOT_FOUND);
        
        var existingJob = existingJobResult.Value;

        return !JobBusinessRules.CanEdit(existingJob.JobStatusId)
            ? Result.Fail<Unit>(JobValidationMessages.CAN_ONLY_EDIT_IN_DRAFT)
            : await unitOfWork.ExecuteInTransactionAsync(async (ct) =>
            {
                request.Job.Adapt(existingJob);

                await UpdateCollectionsAsync(existingJob, request.Job);

               var result = await unitOfWork.SaveChangesAsync(ct);
                return result == 0 ? Result.Fail<Unit>(JobValidationMessages.UPDATE_FAILED) : Result.Ok(Unit.Value);
            }, cancellationToken);
    }

    private async Task UpdateCollectionsAsync(JobEntity job, UpdateJobDto dto)
    {
        await UpdateSkillsAsync(job, dto.Skills!);
        await UpdateConditionsAsync(job, dto.Conditions!);
        await UpdateDegreesAsync(job, dto.Degrees!.Select(d => d.DegreeId).ToList());
        await UpdateResponsibilityAsync(job, dto.Responsibilities!);
        await UpdateRequiredAttachmentsAsync(job, dto.RequiredAttachments!);

        if (job.JobQuota != null)
            await UpdateQuota(job.JobQuota, dto.Quota!);
    }
    
    private async Task UpdateSkillsAsync(JobEntity job, List<JobSkillRequestDto> newSkills)
    {
        var toRemove = job.JobSkills
            .Where(s => !newSkills.Any(newSkill => newSkill.SkillId == s.SkillId))
            .ToList();

        if (toRemove.Count != 0)
        {
            job.JobSkills.RemoveAll(s => toRemove.Any(x => x.Id == s.Id));
            await jobSkillRepository.Repository.DeleteRangeAsync(toRemove);
        }

        var existingSkillIds = job.JobSkills.Select(s => s.SkillId).ToHashSet();
        var newItems = new List<JobSkill>();

        foreach (var skillDto in newSkills.Where(s => !existingSkillIds.Contains(s.SkillId)))
        {
            var item = new JobSkill
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                SkillId = skillDto.SkillId,
                ShowToApplicants = skillDto.ShowToApplicants,
            };

            job.JobSkills.Add(item);
            newItems.Add(item);
        }

        if (newItems.Count != 0)
            await jobSkillRepository.Repository.AddRangeAsync(newItems);
    }
    
    private async Task UpdateConditionsAsync(JobEntity job, List<JobConditionRequestDto> newConditions)
    {
        var toRemove = job.JobConditions
            .Where(c => !newConditions.Any(dto => dto.TextAr == c.TextAr))
            .ToList();

        if (toRemove.Count != 0)
        {
            job.JobConditions.RemoveAll(c => toRemove.Any(x => x.Id == c.Id));
            await jobConditionRepository.Repository.DeleteRangeAsync(toRemove);
        }

        var existingTexts = job.JobConditions.Select(c => c.TextAr).ToHashSet();

        var newItems = new List<JobCondition>();

        foreach (var dto in newConditions.Where(c => !existingTexts.Contains(c.TextAr)))
        {
            var item = new JobCondition
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                TextAr = dto.TextAr,
                TextEn = dto.TextEn,
            };

            job.JobConditions.Add(item);
            newItems.Add(item);
        }

        if (newItems.Count != 0)
            await jobConditionRepository.Repository.AddRangeAsync(newItems);
    }
    
    private async Task UpdateDegreesAsync(JobEntity job, List<Guid> newDegreeIds)
    {
        var toRemove = job.JobDegrees
            .Where(d => !newDegreeIds.Contains(d.DegreeId))
            .ToList();

        if (toRemove.Count != 0)
        {
            job.JobDegrees.RemoveAll(d => toRemove.Any(x => x.Id == d.Id));
            await jobDegreeRepository.Repository.DeleteRangeAsync(toRemove);
        }

        var existingIds = job.JobDegrees.Select(d => d.DegreeId).ToHashSet();
        var newItems = new List<JobDegree>();

        foreach (var id in newDegreeIds.Where(i => !existingIds.Contains(i)))
        {
            var item = new JobDegree
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                DegreeId = id
            };

            job.JobDegrees.Add(item);
            newItems.Add(item);
        }

        if (newItems.Count != 0)
            await jobDegreeRepository.Repository.AddRangeAsync(newItems);
    }
    
    private async Task UpdateQuota(JobQuota quota, JobQuotaRequestDto? dto)
    {
        quota.QatariCitizens = dto!.QatariCitizens;
        quota.QatarMother = dto.QatarMother;
        quota.NonQatariSpouse = dto.NonQatariSpouse;
        quota.Gcc = dto.Gcc;
        quota.QuGrads = dto.QuGrads;
        quota.Residents = dto.Residents;

        await UpdateResidentsBreakdownAsync(quota, dto!.ResidentsBreakdowns);
    }
    
    private async Task UpdateResidentsBreakdownAsync(JobQuota quota, List<ResidentBreakdownRequestDto>? dtos)
    {
        var dtoIds = dtos!.Select(d => d.NationalityId).ToHashSet();

        var toRemove = quota.ResidentsBreakdowns
            .Where(rb => !dtoIds.Contains(rb.NationalityId))
            .ToList();

        if (toRemove.Count != 0)
        {
            quota.ResidentsBreakdowns.RemoveAll(rb => toRemove.Any(x => x.Id == rb.Id));
            await residentBreakdownRepository.Repository.DeleteRangeAsync(toRemove);
        }

        var existing = quota.ResidentsBreakdowns.ToDictionary(rb => rb.NationalityId, rb => rb);
        var newItems = new List<ResidentBreakdown>();

        foreach (var dto in dtos!)
        {
            if (existing.TryGetValue(dto.NationalityId, out var record))
            {
                record.Percentage = dto.Percentage;
                await residentBreakdownRepository.Repository.UpdateAsync(record);
            }
            else
            {
                var item = new ResidentBreakdown
                {
                    Id = Guid.NewGuid(),
                    JobQuotaId = quota.Id,
                    NationalityId = dto.NationalityId,
                    Percentage = dto.Percentage
                };

                quota.ResidentsBreakdowns.Add(item);
                newItems.Add(item);
            }
        }

        if (newItems.Count != 0)
            await residentBreakdownRepository.Repository.AddRangeAsync(newItems);
    }
    
    private Task UpdateResponsibilityAsync(JobEntity job, List<JobResponsibilityRequestDto> newResponsibilities)
    {
        var toRemove = job.JobResponsibilities
            .Where(r => !newResponsibilities.Any(dto =>
                !string.IsNullOrWhiteSpace(dto.TextAr) &&
                string.Equals(dto.TextAr, r.TitleAr, StringComparison.Ordinal)))
            .ToList();

        if (toRemove.Count != 0)
        {
            job.JobResponsibilities.RemoveAll(r => toRemove.Any(x => x.Id == r.Id));
        }

        var existingIds = job.JobResponsibilities.Select(r => r.Id).ToHashSet();
        var newItems = new List<JobResponsibility>();

        foreach (var dto in newResponsibilities.Where(r => !string.IsNullOrWhiteSpace(r.TextEn)))
        {
            var item = new JobResponsibility
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                TitleAr = dto.TextAr,
                TitleEn = dto.TextEn,
            };

            job.JobResponsibilities.Add(item);
            newItems.Add(item);
        }

        return Task.CompletedTask;
    }

    private Task UpdateRequiredAttachmentsAsync(JobEntity job, List<JobRequiredAttachmentRequestDto> newAttachments)
    {
        var existingTitlesAr = job.JobRequiredAttachments.Select(a => a.TitleAr).ToHashSet();
        var existingTitlesEn = job.JobRequiredAttachments.Select(a => a.TitleEn).ToHashSet();

        var toRemove = job.JobRequiredAttachments
        .Where(a => !newAttachments.Any(dto =>
            dto.TitleAr == a.TitleAr || dto.TitleEn == a.TitleEn))
        .ToList();

        if (toRemove.Count != 0)
        {
            job.JobRequiredAttachments.RemoveAll(a => toRemove.Any(x => x.Id == a.Id));
        }


        var newItems = new List<JobRequiredAttachment>();

        foreach (var dto in newAttachments.Where(a =>
        !existingTitlesAr.Contains(a.TitleAr) && !existingTitlesEn.Contains(a.TitleEn)))
        {
            var item = new JobRequiredAttachment
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                TitleAr = dto.TitleAr,
                TitleEn = dto.TitleEn,
                IsMandatory = dto.IsMandatory
            };

            job.JobRequiredAttachments.Add(item);
            newItems.Add(item);
        }
        
        return Task.CompletedTask;

    }
}
