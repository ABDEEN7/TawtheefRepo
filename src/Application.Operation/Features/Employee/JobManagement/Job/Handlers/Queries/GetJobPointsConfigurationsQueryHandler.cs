using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.Job.DTOs;
using Application.Operation.Features.Employee.Job.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;

namespace Application.Operation.Features.Employee.Job.Handlers.Queries;

public class GetJobPointsConfigurationsQueryHandler(IJobPointsConfigurationsRepository jobPointsConfigRepository, IMapper mapper)
    : IQueryHandler<GetJobPointsConfigurationsQuery, IResult<JobPointConfigurationResponseDto>>
{

    public async Task<IResult<JobPointConfigurationResponseDto>> Handle(GetJobPointsConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var result = await jobPointsConfigRepository.GetAsync();
        if (result.IsFailed)
            return Result.Fail<JobPointConfigurationResponseDto>(result.Errors);
        var jobPoints = result.Value;

        var jobDto = mapper.Map<JobPointConfigurationResponseDto>(jobPoints);

        return Result.Ok(jobDto);
    }

}
