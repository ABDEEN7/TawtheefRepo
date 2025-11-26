using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class UpdateJobCommandHandler(
    IJobRepository jobRepository,
    IJobQuotaRepository jobQuotaRepository,
    IJobDegreeRepository jobDegreeRepository,
    IJobConditionRepository jobConditionRepository,
    IJobSkillRepository jobSkillRepository,
    IResidentsBreakdownRepository residentsBreakdownRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        var existingJobResult = await jobRepository.GetByIdWithDetailsAsync(request.Job.Id);
        if (existingJobResult.IsFailed)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Fail<Unit>($"{JobValidationMessages.JobNotFound}: {existingJobResult.Errors}");
        }
        
        var existingJob = existingJobResult.Value;
        if (existingJob == null)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Fail<Unit>(JobValidationMessages.JobNotFound);
        }
        // Begin transaction
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Update main job properties
            UpdateMainJobProperties(existingJob, request.Job);
            
            // Update collections using repositories for better control
            var collectionsResult = await UpdateJobCollectionsAsync(existingJob, request.Job);
            if (collectionsResult.IsFailed)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Fail<Unit>(collectionsResult.Errors);
            }

            // Update job status
            existingJob.StatusId = JobStatusIds.Draft;

            // Update the main job entity
            var updateResult = await jobRepository.Repository.UpdateAsync(existingJob);
            if (updateResult.IsFailed)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Fail<Unit>(updateResult.Errors);
            }

            // Save changes and commit
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Ok(Unit.Value);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Fail<Unit>($"{JobValidationMessages.UpdateFailed}: {ex.Message}");
        }
    }

    private void UpdateMainJobProperties(JobEntity existingJob, UpdateJobDto updateDto)
    {
        existingJob.Title = updateDto.Title;
        existingJob.Vacancies = updateDto.Vacancies;
        existingJob.Deadline = updateDto.Deadline;
        existingJob.Description = updateDto.Description;
        existingJob.Benefits = updateDto.Benefits;
        existingJob.RequestingDepartmentId = updateDto.RequestingDepartmentId;
        existingJob.JobCategoryId = updateDto.JobCategoryId;
        existingJob.GenderId = updateDto.GenderId;
        existingJob.WorkLocationId = updateDto.WorkLocationId;
        existingJob.MajorId = updateDto.MajorId;
        existingJob.WorkTypeId = updateDto.WorkTypeId;
        existingJob.StatusId = updateDto.StatusId;
    }

    private async Task<IResult<Unit>> UpdateJobCollectionsAsync(JobEntity existingJob, UpdateJobDto updateDto)
    {
        // Update skills
        var skillsResult = await UpdateSkillsAsync(existingJob.Id, updateDto.Skills);
        if (skillsResult.IsFailed)
            return Result.Fail<Unit>(skillsResult.Errors);

        // Update conditions
        var conditionsResult = await UpdateConditionsAsync(existingJob.Id, updateDto.Conditions);
        if (conditionsResult.IsFailed)
            return Result.Fail<Unit>(conditionsResult.Errors);

        // Update degrees
        var degreesResult = await UpdateDegreesAsync(existingJob.Id, updateDto.DegreeIds);
        if (degreesResult.IsFailed)
            return Result.Fail<Unit>(degreesResult.Errors);

        // Update quotas

            var quotaResult = await UpdateQuotasAsync(existingJob.QuotaId, updateDto.Quota);
            return quotaResult.IsFailed ? Result.Fail<Unit>(quotaResult.Errors) : Result.Ok(Unit.Value);
    }

    private async Task<IResult<Unit>> UpdateSkillsAsync(Guid jobId, List<string> skills)
    {
        // Remove existing skills
        var existingSkillsResult = await jobSkillRepository.GetByJobIdAsync(jobId);
        if (existingSkillsResult.IsSuccess)
        {
            var jobSkills = existingSkillsResult.Value.Where(s => s.JobId == jobId).ToList();
            foreach (var skill in jobSkills)
            {
                var deleteResult = await jobSkillRepository.Repository.DeleteAsync(skill);
                if (deleteResult.IsFailed)
                    return Result.Fail<Unit>(deleteResult.Errors);
            }
        }

        // Add new skills
        if (skills.Count != 0)
        {
            var order = 1;
            var newSkills = skills.Select(skill => new JobSkill { Id = Guid.NewGuid(), JobId = jobId, Text = skill, Order = order++ }).ToList();

            var addResult = await jobSkillRepository.Repository.AddRangeAsync(newSkills);
            if (addResult.IsFailed)
                return Result.Fail<Unit>(addResult.Errors);
        }

        return Result.Ok(Unit.Value);
    }

    private async Task<IResult<Unit>> UpdateConditionsAsync(Guid jobId, List<string> conditions)
    {
        // Remove existing conditions
        var existingConditionsResult = await jobConditionRepository.GetByJobIdAsync(jobId);
        if (existingConditionsResult.IsSuccess)
        {
            var jobConditions = existingConditionsResult.Value.Where(c => c.JobId == jobId).ToList();
            foreach (var condition in jobConditions)
            {
                var deleteResult = await jobConditionRepository.Repository.DeleteAsync(condition);
                if (deleteResult.IsFailed)
                    return Result.Fail<Unit>(deleteResult.Errors);
            }
        }

        // Add new conditions
        if (conditions.Count != 0)
        {
            var order = 1;
            var newConditions = conditions.Select(condition => new JobCondition { Id = Guid.NewGuid(), JobId = jobId, Text = condition, Order = order++ }).ToList();

            var addResult = await jobConditionRepository.Repository.AddRangeAsync(newConditions);
            if (addResult.IsFailed)
                return Result.Fail<Unit>(addResult.Errors);
        }

        return Result.Ok(Unit.Value);
    }

    private async Task<IResult<Unit>> UpdateDegreesAsync(Guid jobId, List<Guid> degreeIds)
    {
        // Remove existing degrees
        var existingDegreesResult = await jobDegreeRepository.GetByJobIdAsync(jobId);
        if (existingDegreesResult.IsSuccess)
        {
            var jobDegrees = existingDegreesResult.Value.Where(d => d.JobId == jobId).ToList();
            foreach (var degree in jobDegrees)
            {
                var deleteResult = await jobDegreeRepository.Repository.DeleteAsync(degree);
                if (deleteResult.IsFailed)
                    return Result.Fail<Unit>(deleteResult.Errors);
            }
        }

        // Add new degrees
        if (degreeIds.Count != 0)
        {
            var newDegrees = degreeIds.Select(degreeId => new JobDegree
            {
                Id = Guid.NewGuid(),
                JobId = jobId,
                DegreeId = degreeId
            }).ToList();

            var addResult = await jobDegreeRepository.Repository.AddRangeAsync(newDegrees);
            if (addResult.IsFailed)
                return Result.Fail<Unit>(addResult.Errors);
        }

        return Result.Ok(Unit.Value);
    }

    private async Task<IResult<Unit>> UpdateQuotasAsync(Guid? quotaId, JobQuotaDto updateQuotas)
    {
        if (!quotaId.HasValue)
            return Result.Fail<Unit>(JobValidationMessages.QuotaIdRequired);

        var existingQuotaResult = await jobQuotaRepository.Repository.GetByIdAsync(quotaId.Value);
        if (existingQuotaResult.IsFailed || existingQuotaResult.Value == null)
            return Result.Fail<Unit>(JobValidationMessages.QuotaNotFound);

        var existingQuota = existingQuotaResult.Value;

        // Update quota properties
        existingQuota.QatariCitizens = updateQuotas.QatariCitizens;
        existingQuota.QatarMother = updateQuotas.QatarMother;
        existingQuota.NonQatariSpouse = updateQuotas.NonQatariSpouse;
        existingQuota.Gcc = updateQuotas.Gcc;
        existingQuota.QuGrads = updateQuotas.QuGrads;
        existingQuota.Residents = updateQuotas.Residents;

        // Update residents breakdown
        await UpdateResidentsBreakdownAsync(existingQuota, updateQuotas.ResidentsBreakdowns);

        var updateResult = await jobQuotaRepository.Repository.UpdateAsync(existingQuota);
        return updateResult.IsSuccess 
            ? Result.Ok(Unit.Value) 
            : Result.Fail<Unit>(updateResult.Errors);
    }

    private async Task UpdateResidentsBreakdownAsync(JobQuota quota, List<ResidentBreakdownDto> residentsBreakdownDtos)
    {
        var existingBreakdowns = quota.ResidentsBreakdowns.ToList();
        foreach (var breakdown in existingBreakdowns)
        {
            await residentsBreakdownRepository.Repository.DeleteAsync(breakdown); // Assuming your repo supports this
        }

        if (residentsBreakdownDtos.Count != 0)
        {
            foreach (var breakdownDto in residentsBreakdownDtos)
            {
                quota.ResidentsBreakdowns.Add(new ResidentBreakdown
                {
                    Id = Guid.NewGuid(),
                    JobQuotaId = quota.Id,
                    NationalityId = breakdownDto.NationalityId,
                    Percentage = breakdownDto.Percentage
                });
            }
        }
    }
}
