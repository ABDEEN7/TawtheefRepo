using Mapster;
using FluentResults;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobByIdQueryHandler(IJobRepository jobRepository,IMapper mapper)
    : IRequestHandler<GetJobByIdQuery, IResult<JobResponseDto>>
{
    public async Task<IResult<JobResponseDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await jobRepository.GetByIdWithDetailsAsync(request.jobId);

        if (result.IsFailed)
            return Result.Fail<JobResponseDto>(result.Errors);

        var job = result.Value;

        if (job is null)
            return Result.Fail<JobResponseDto>(JobValidationMessages.JOB_NOT_FOUND);
        
        var jobDto = mapper.Map<JobResponseDto>(job);

        return Result.Ok(jobDto);
    }
}
