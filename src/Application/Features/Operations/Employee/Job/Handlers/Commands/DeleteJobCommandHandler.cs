using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class DeleteJobCommandHandler(IJobRepository jobRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteJobCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
    {

            var existingJobResult = await jobRepository.Repository.GetByIdAsync(request.JobId);
            if (existingJobResult.IsFailed)
                return Result.Fail<Unit>(JobValidationMessages.JOB_NOT_FOUND);

            var existingJob = existingJobResult.Value;

            if (existingJob != null)
            {
                var deleteResult = await jobRepository.Repository.DeleteAsync(existingJob);
                if (deleteResult.IsFailed)
                    return Result.Fail<Unit>(deleteResult.Errors);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(Unit.Value);  
    }
    
}
