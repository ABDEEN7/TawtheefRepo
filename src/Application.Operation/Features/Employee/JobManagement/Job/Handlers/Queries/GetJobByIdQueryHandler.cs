using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Application.Operation.Features.Employee.JobManagement.Job.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Queries;

public class GetJobByIdQueryHandler(IJobRepository jobRepository, IMapper mapper)
    : IRequestHandler<GetJobByIdQuery, IResult<JobResponseDto>>
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

