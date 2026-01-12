using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Application.Operation.Features.Employee.JobManagement.Job.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Queries;

public class GetJobPointsByJobIdQueryHandler(IJobPointsRepository jobPointsRepository, IMapper mapper)
    : IQueryHandler<GetJobPointsByJobIdQuery, IResult<JobPointsMainResponseDto>>
{

    public async Task<IResult<JobPointsMainResponseDto>> Handle(GetJobPointsByJobIdQuery request, CancellationToken cancellationToken)
    {
        var result = await jobPointsRepository.GetByJobIdAsync(request.JobId);
        if(result.IsFailed)
            return Result.Fail<JobPointsMainResponseDto>(result.Errors);
        var jobPoints = result.Value;

        var jobDto = mapper.Map<JobPointsMainResponseDto>(jobPoints);

        return Result.Ok(jobDto);
    }

}
