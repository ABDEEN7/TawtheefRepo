using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobByIdQueryHandler(IJobRepository jobRepository)
    : IRequestHandler<GetJobByIdQuery, IResult<JobResponseDto>>
{
    public async Task<IResult<JobResponseDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await jobRepository.GetByIdWithDetailsAsync(request.JobId);

        if (result.IsFailed)
            return Result.Fail<JobResponseDto>(result.Errors);

        var job = result.Value;

        if (job is null)
            return Result.Fail<JobResponseDto>(JobValidationMessages.JOB_NOT_FOUND);
        var jobDto = JobManualMapper.Map(job);
        return Result.Ok(jobDto);
    }
}
