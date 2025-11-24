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

public class UpdateJobCommandHandler(IJobRepository jobRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
            var existingJobResult = await jobRepository.Repository.GetByIdAsync(request.JobId);
            if (existingJobResult.IsFailed)
                return Result.Fail<Unit>($"{JobValidationMessages.JobNotFound}: {existingJobResult.Errors}");

            var existingJob = existingJobResult.Value;
            
            // Update main job properties
            if (existingJob != null)
            {
                UpdateMainJobProperties(existingJob, request.Job);

                // Update owned type - JobBasics
                UpdateJobBasics(existingJob, request.Job.Basics);

                // Update collections (skills, conditions, degrees, quotas)
                UpdateJobCollections(existingJob, request.Job);

                // Update job status if needed (e.g., back to draft if modified)
                existingJob.StatusId = JobStatusIds.Draft;

                var updateResult = await jobRepository.Repository.UpdateAsync(existingJob);
                if (updateResult.IsFailed)
                    return Result.Fail<Unit>(updateResult.Errors);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(Unit.Value);

    }

    private void UpdateMainJobProperties(JobEntity existingJob, UpdateJobDto updateDto)
    {
        existingJob.Description = updateDto.Description;
        existingJob.Benefits = updateDto.Benefits;
        
        // Update navigation property IDs through the Basics owned type
        existingJob.RequestingDepartmentId = updateDto.Basics.RequestingDeptId;
        existingJob.JobCategoryId = updateDto.Basics.JobCategoryId;
        existingJob.GenderId = updateDto.Basics.GenderId;
        existingJob.TargetEntityId = updateDto.Basics.TargetEntityId;
        existingJob.MajorId = updateDto.Basics.MajorId;
        existingJob.WorkTypeId = updateDto.Basics.WorkTypeId;
    }

    private void UpdateJobBasics(JobEntity existingBasics, JobBasicsDto updateBasics)
    {
        existingBasics.Title = updateBasics.Title;
        existingBasics.Vacancies = updateBasics.Vacancies;
        existingBasics.Deadline = updateBasics.Deadline;
    }

    private void UpdateJobCollections(JobEntity existingJob, UpdateJobDto updateDto)
    {
        // Update skills - clear and add new
        existingJob.Skills.Clear();
        var skillOrder = 1;
        foreach (var skill in updateDto.Skills)
        {
            existingJob.Skills.Add(new JobSkill
            {
                Id = Guid.NewGuid(),
                JobId = existingJob.Id,
                Text = skill,
                Order = skillOrder++
            });
        }

        // Update conditions - clear and add new
        existingJob.Conditions.Clear();
        var conditionOrder = 1;
        foreach (var condition in updateDto.Conditions)
        {
            existingJob.Conditions.Add(new JobCondition
            {
                Id = Guid.NewGuid(),
                JobId = existingJob.Id,
                Text = condition,
                Order = conditionOrder++
            });
        }

        // Update degrees - clear and add new
        existingJob.Degrees.Clear();
        foreach (var degreeId in updateDto.DegreeIds)
        {
            existingJob.Degrees.Add(new JobDegree
            {
                Id = Guid.NewGuid(),
                JobId = existingJob.Id,
                DegreeId = degreeId
            });
        }

        // Update quotas
        UpdateJobQuotas(existingJob.Quotas, updateDto.Quotas);
    }

    private void UpdateJobQuotas(JobQuotas existingQuotas, JobQuotasDto updateQuotas)
    {
        existingQuotas.QatariCitizens = updateQuotas.QatariCitizens;
        existingQuotas.QatarMother = updateQuotas.QatarMother;
        existingQuotas.NonQatariSpouse = updateQuotas.NonQatariSpouse;
        existingQuotas.Gcc = updateQuotas.Gcc;
        existingQuotas.QuGrads = updateQuotas.QuGrads;
        existingQuotas.Residents = updateQuotas.Residents;

        // Update residents breakdown - clear and add new
        existingQuotas.ResidentsBreakdown.Clear();
        foreach (var breakdownDto in updateQuotas.ResidentsBreakdown)
        {
            existingQuotas.ResidentsBreakdown.Add(new ResidentBreakdown
            {
                Id = Guid.NewGuid(),
                JobQuotaId = existingQuotas.Id,
                NationalityId = breakdownDto.NationalityId,
                Percentage = breakdownDto.Percentage
            });
        }
    }
}
