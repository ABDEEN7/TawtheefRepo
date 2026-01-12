using Application.Operation.Features.Employee.Job.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Features.Employee.Job.Handlers.Commands;

public class DeleteJobCommandHandler(IJobRepository jobRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteJobCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
    {

            var existingJobResult = await jobRepository.Repository.GetByIdAsync(request.JobId);
            if (existingJobResult.IsFailed)
                return Result.Fail<Unit>(JobMessages.JobNotFound);

            var existingJob = existingJobResult.Value;

            if (existingJob != null)
            {
                existingJob.AddDomainEvent(new JobDeletedDomainEvent(existingJob, DateTimeOffset.UtcNow));
                var deleteResult = await jobRepository.Repository.DeleteAsync(existingJob);
                if (deleteResult.IsFailed)
                    return Result.Fail<Unit>(deleteResult.Errors);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(Unit.Value);  
    }
    
}
