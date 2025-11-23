using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class DeleteJobCommandHandler(IJobRepository jobRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteJobCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing job
            var existingJobResult = await jobRepository.GetByIdAsync(request.JobId);
            if (existingJobResult.IsFailed)
                return Result.Fail<Unit>($"Job not found: {existingJobResult.Errors}");

            var existingJob = existingJobResult.Value;

            // Perform deletion
            var deleteResult = await jobRepository.Repository.DeleteAsync(existingJob);
            if (deleteResult.IsFailed)
                return Result.Fail<Unit>(deleteResult.Errors);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result.Fail<Unit>($"Failed to delete job: {ex.Message}");
        }
    }
    
}
