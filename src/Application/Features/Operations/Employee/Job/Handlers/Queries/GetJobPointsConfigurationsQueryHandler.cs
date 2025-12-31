using FluentResults;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobPointsConfigurationsQueryHandler(IJobPointsConfigurationsRepository jobPointsConfigRepository, IMapper mapper)
    : IRequestHandler<GetJobPointsConfigurationsQuery, IResult<JobPointConfigurationResponseDto>>
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
