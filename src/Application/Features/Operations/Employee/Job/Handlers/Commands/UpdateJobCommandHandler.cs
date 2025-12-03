using FluentResults;
using Mapster;
using MediatR;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
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
        var existingJobResult = await jobRepository.GetByIdWithDetailsAsync(request.Job.Id);
        if (existingJobResult.IsFailed || existingJobResult.Value == null)
            return Result.Fail<Unit>(JobValidationMessages.JOB_NOT_FOUND);
        
        var existingJob = existingJobResult.Value;
            return await unitOfWork.ExecuteInTransactionAsync(async (ct) =>
            {
                request.Job.Adapt(existingJob);
                
                await UpdateCollectionsAsync(existingJob, request.Job);
                
               var result = await unitOfWork.SaveChangesAsync(ct);
                return result == 0 ? Result.Fail<Unit>(JobValidationMessages.UPDATE_FAILED) : Result.Ok(Unit.Value);
            }, cancellationToken);

    }
    
    private async Task UpdateCollectionsAsync(JobEntity job, UpdateJobDto dto)
    {
        await UpdateSkillsAsync(job, dto.Skills);
        await UpdateConditionsAsync(job, dto.Conditions);
        await UpdateDegreesAsync(job, dto.Degrees.Select(d => d.DegreeId).ToList());
        await UpdateResponsibilityAsync(job, dto.Responsibilities);
        await UpdateRequiredAttachmentsAsync(job, dto.RequiredAttachments);

        if (job.Quota != null)
            await UpdateQuota(job.Quota, dto.Quota);
    }
    
    private async Task UpdateSkillsAsync(JobEntity job, List<JobSkillDto> newSkills)
    {
        var toRemove = job.Skills
            .Where(s => !newSkills.Any(newSkill => newSkill.SkillId == s.SkillId))
            .ToList();

        if (toRemove.Count != 0)
        {
            job.Skills.RemoveAll(s => toRemove.Any(x => x.Id == s.Id));
            await jobSkillRepository.Repository.DeleteRangeAsync(toRemove);
        }

        var existingSkillIds = job.Skills.Select(s => s.SkillId).ToHashSet();
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

            job.Skills.Add(item);
            newItems.Add(item);
        }

        if (newItems.Count != 0)
            await jobSkillRepository.Repository.AddRangeAsync(newItems);
    }
    
    private async Task UpdateConditionsAsync(JobEntity job, List<JobConditionDto> newConditions)
    {
        var toRemove = job.Conditions
            .Where(c => !newConditions.Any(dto => dto.Text == c.Text))
            .ToList();

        if (toRemove.Count != 0)
        {
            job.Conditions.RemoveAll(c => toRemove.Any(x => x.Id == c.Id));
            await jobConditionRepository.Repository.DeleteRangeAsync(toRemove);
        }

        var existingTexts = job.Conditions.Select(c => c.Text).ToHashSet();

        var newItems = new List<JobCondition>();

        foreach (var dto in newConditions.Where(c => !existingTexts.Contains(c.Text)))
        {
            var item = new JobCondition
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                Text = dto.Text,
            };

            job.Conditions.Add(item);
            newItems.Add(item);
        }

        if (newItems.Count != 0)
            await jobConditionRepository.Repository.AddRangeAsync(newItems);
    }
    
    private async Task UpdateDegreesAsync(JobEntity job, List<Guid> newDegreeIds)
    {
        var toRemove = job.Degrees
            .Where(d => !newDegreeIds.Contains(d.DegreeId))
            .ToList();

        if (toRemove.Count != 0)
        {
            job.Degrees.RemoveAll(d => toRemove.Any(x => x.Id == d.Id));
            await jobDegreeRepository.Repository.DeleteRangeAsync(toRemove);
        }

        var existingIds = job.Degrees.Select(d => d.DegreeId).ToHashSet();
        var newItems = new List<JobDegree>();

        foreach (var id in newDegreeIds.Where(i => !existingIds.Contains(i)))
        {
            var item = new JobDegree
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                DegreeId = id
            };

            job.Degrees.Add(item);
            newItems.Add(item);
        }

        if (newItems.Count != 0)
            await jobDegreeRepository.Repository.AddRangeAsync(newItems);
    }
    
    private async Task UpdateQuota(JobQuota quota, JobQuotaDto dto)
    {
        quota.QatariCitizens = dto.QatariCitizens;
        quota.QatarMother = dto.QatarMother;
        quota.NonQatariSpouse = dto.NonQatariSpouse;
        quota.Gcc = dto.Gcc;
        quota.QuGrads = dto.QuGrads;
        quota.Residents = dto.Residents;

        await UpdateResidentsBreakdownAsync(quota, dto.ResidentsBreakdowns);
    }
    
    private async Task UpdateResidentsBreakdownAsync(JobQuota quota, List<ResidentBreakdownDto> dtos)
    {
        var dtoIds = dtos.Select(d => d.NationalityId).ToHashSet();

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

        foreach (var dto in dtos)
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
    
    private Task UpdateResponsibilityAsync(JobEntity job, List<JobResponsibilityDto> newResponsibilities)
    {
        var toRemove = job.Responsibilities
            .Where(r => !newResponsibilities.Any(dto => dto.Id.HasValue && dto.Id.Value == r.Id))
            .ToList();

        if (toRemove.Count != 0)
        {
            job.Responsibilities.RemoveAll(r => toRemove.Any(x => x.Id == r.Id));
        }

        var existingIds = job.Responsibilities.Select(r => r.Id).ToHashSet();
        var newItems = new List<JobResponsibility>();

        foreach (var dto in newResponsibilities.Where(r => !r.Id.HasValue || !existingIds.Contains(r.Id.Value)))
        {
            var item = new JobResponsibility
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                Text = dto.Text
            };

            job.Responsibilities.Add(item);
            newItems.Add(item);
        }
        
        return Task.CompletedTask;
    }

    private Task UpdateRequiredAttachmentsAsync(JobEntity job, List<RequiredAttachmentDto> newAttachments)
    {
        var toRemove = job.RequiredAttachments
            .Where(a => !newAttachments.Any(dto => dto.Title == a.Title))
            .ToList();

        if (toRemove.Count != 0)
        {
            job.RequiredAttachments.RemoveAll(a => toRemove.Any(x => x.Id == a.Id));
        }

        var existingTitles = job.RequiredAttachments.Select(a => a.Title).ToHashSet();
        var newItems = new List<JobRequiredAttachment>();

        foreach (var dto in newAttachments.Where(a => !existingTitles.Contains(a.Title)))
        {
            var item = new JobRequiredAttachment
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                Title = dto.Title,
                IsMandatory = dto.IsMandatory
            };

            job.RequiredAttachments.Add(item);
            newItems.Add(item);
        }
        
        return Task.CompletedTask;

    }
}
