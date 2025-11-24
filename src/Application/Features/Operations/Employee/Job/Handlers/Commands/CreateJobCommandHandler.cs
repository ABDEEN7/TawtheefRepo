using Mapster;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Domain.Entities.Lookups;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class CreateJobCommandHandler(IJobRepository jobRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateJobCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
    
            var job = request.Job.Adapt<JobEntity>();
            
            job.Id = Guid.NewGuid();
            job.StatusId = JobStatusIds.Draft;
            
            
            SetCollectionIds(job);
            var addResult = await jobRepository.Repository.AddAsync(job);
            if (addResult.IsFailed)
                return Result.Fail<Guid>(addResult.Errors);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(job.Id);
    }

    private void SetCollectionIds(JobEntity job)
    {
        // Set IDs for skills
        foreach (var skill in job.Skills)
        {
            skill.Id = Guid.NewGuid();
            skill.JobId = job.Id;
        }

        // Set IDs for conditions
        foreach (var condition in job.Conditions)
        {
            condition.Id = Guid.NewGuid();
            condition.JobId = job.Id;
        }

        // Set IDs for degrees
        foreach (var degree in job.Degrees)
        {
            degree.Id = Guid.NewGuid();
            degree.JobId = job.Id;
        }

        // Set IDs for quotas and breakdowns
        job.Quota?.Id = Guid.NewGuid();
        if (job.Quota == null)
            return;

        foreach (var breakdown in job.Quota.ResidentsBreakdowns)
        {
            breakdown.Id = Guid.NewGuid();
            breakdown.JobQuotaId = job.Quota.Id;
        }
    }
}
