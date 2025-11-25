using FluentResults;
using Mapster;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class CreateJobCommandHandler(
    IJobRepository jobRepository,
    IJobQuotaRepository jobQuotaRepository,
    IJobDegreeRepository jobDegreeRepository,
    IJobConditionRepository jobConditionRepository,
    IJobSkillRepository jobSkillRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateJobCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
            // Begin transaction
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            
                // 1. Create main job entity
                var job = request.Job.Adapt<JobEntity>();
                
                // 2. Create and save quota
                var quotaResult = await CreateJobQuotaAsync(request, job);
                if (quotaResult.IsFailed)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return quotaResult;
                }
                
                // 3. Save the main job
                var jobResult = await jobRepository.Repository.AddAsync(job);
                if (jobResult.IsFailed)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Fail<Guid>(jobResult.Errors);
                }
                
                // 4. Create related entities
                var relatedEntitiesResult = await CreateRelatedEntitiesAsync(job.Id, request.Job);
                if (relatedEntitiesResult.IsFailed)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return relatedEntitiesResult;
                }
                
                // 5. Save all changes and commit transaction
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Ok(job.Id);

    }
    
    private async Task<IResult<Guid>> CreateJobQuotaAsync(CreateJobCommand request, JobEntity job)
    {
        var quota = (request.Job, job.QuotaId).Adapt<JobQuota>();
        var quotaResult = await jobQuotaRepository.Repository.AddAsync(quota);
        return quotaResult.IsSuccess 
            ? Result.Ok(Guid.Empty) 
            : Result.Fail<Guid>(quotaResult.Errors);
    }
    
    private async Task<IResult<Guid>> CreateRelatedEntitiesAsync(Guid jobId, CreateJobDto jobDto)
    {
        // Create degrees
        if (jobDto.DegreeIds.Any())
        {
            var degrees = (jobId, jobDto.DegreeIds).Adapt<List<JobDegree>>();
            var degreesResult = await jobDegreeRepository.Repository.AddRangeAsync(degrees);
            if (degreesResult.IsFailed) return Result.Fail<Guid>(degreesResult.Errors);
        }

        // Create conditions
        if (jobDto.Conditions.Count != 0)
        {
            var conditions = (jobId, jobDto.Conditions).Adapt<List<JobCondition>>();
            var conditionsResult = await jobConditionRepository.Repository.AddRangeAsync(conditions);
            if (conditionsResult.IsFailed)
                return Result.Fail<Guid>(conditionsResult.Errors);
        }
        
        // Create skills
        if (jobDto.Skills.Any())
        {
            var skills = (jobId, jobDto.Skills).Adapt<List<JobSkill>>();
            var skillsResult = await jobSkillRepository.Repository.AddRangeAsync(skills);
            if (skillsResult.IsFailed)
                return Result.Fail<Guid>(skillsResult.Errors);
        }
        
        return Result.Ok(Guid.Empty);
    }
}
