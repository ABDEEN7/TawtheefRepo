using FluentResults;
using Mapster;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;
public class GetLatestJobTabReviewsQueryHandler(IJobTabReviewNoteRepository jobTabReviewRepository,IMediaUrlResolver media, IMapper mapper)
    : IRequestHandler<GetLatestJobTabReviewsQuery, IResult<List<JobTabReviewNoteResponseDto>>>
{
    public async Task<IResult<List<JobTabReviewNoteResponseDto>>> Handle(GetLatestJobTabReviewsQuery request, CancellationToken cancellationToken)
    {
        var result = await jobTabReviewRepository.GetLastReviewCycleAsync(request.jobId);

        if (result.IsFailed)
            return Result.Fail<List<JobTabReviewNoteResponseDto>>(result.Errors);

        var job = result.Value;

        if (job is null)
            return Result.Fail<List<JobTabReviewNoteResponseDto>>(JobMessages.JOB_NOT_FOUND);

        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;
        var jobDto = mapper.Map<List<JobTabReviewNoteResponseDto>>(job);
        return Result.Ok(jobDto);
    }
}
