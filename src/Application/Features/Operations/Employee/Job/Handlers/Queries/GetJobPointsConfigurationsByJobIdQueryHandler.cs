using FluentResults;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobPointsConfigurationsByJobIdQueryHandler(IJobPointsConfigurationsRepository jobPointsConfigRepository, IMapper mapper)
    : IRequestHandler<GetJobPointsConfigurationsByJobIdQuery, IResult<JobPointConfigurationResponseDto>>
{

    public async Task<IResult<JobPointConfigurationResponseDto>> Handle(GetJobPointsConfigurationsByJobIdQuery request, CancellationToken cancellationToken)
    {
        var result = await jobPointsConfigRepository.GetByJobIdAsync(request.JobId);
        if (result.IsFailed)
            return Result.Fail<JobPointConfigurationResponseDto>(result.Errors);
        var jobPoints = result.Value;

        var jobDto = mapper.Map<JobPointConfigurationResponseDto>(jobPoints);

        return Result.Ok(jobDto);
    }

}
