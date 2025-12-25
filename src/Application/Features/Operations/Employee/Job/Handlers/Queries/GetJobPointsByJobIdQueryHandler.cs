using FluentResults;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobPointsByJobIdQueryHandler(IJobPointsRepository jobPointsRepository, IMapper mapper)
    : IRequestHandler<GetJobPointsByJobIdQuery, IResult<JobPointsMainResponseDto>>
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
