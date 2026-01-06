using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobByIdQueryHandler(IJobRepository jobRepository, IMapper mapper)
    : IQueryHandler<GetJobByIdQuery, IResult<JobResponseDto>>
{
    public async Task<IResult<JobResponseDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await jobRepository.GetByIdWithDetailsAsync(request.JobId);

        if (result.IsFailed)
            return Result.Fail<JobResponseDto>(result.Errors);

        var job = result.Value;

        if (job is null)
            return Result.Fail<JobResponseDto>(JobMessages.JobNotFound);

        var jobDto = mapper.Map<JobResponseDto>(job);
        return Result.Ok(jobDto);
    }
}
