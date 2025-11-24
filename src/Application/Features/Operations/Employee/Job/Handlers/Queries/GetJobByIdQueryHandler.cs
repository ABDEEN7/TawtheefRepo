using Mapster;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobByIdQueryHandler(IJobRepository jobRepository)
    : IRequestHandler<GetJobByIdQuery, IResult<JobResponseDto>>
{
    public async Task<IResult<JobResponseDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await jobRepository.GetByIdWithDetailsAsync(request.jobId);
        
        if (result.IsFailed)
            return Result.Fail<JobResponseDto>(result.Errors);

        var job = result.Value;
        
        if (job == null)
            return Result.Fail<JobResponseDto>(JobValidationMessages.JobNotFound);

        var jobDto = job.Adapt<JobResponseDto>();
        
        return Result.Ok(jobDto);
    }
}
