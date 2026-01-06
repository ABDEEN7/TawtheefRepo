using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;
public class GetLatestJobTabReviewsQueryHandler(IJobTabReviewNoteRepository jobTabReviewRepository, IMapper mapper)
    : IQueryHandler<GetLatestJobTabReviewsQuery, IResult<List<JobTabReviewNoteResponseDto>>>
{
    public async Task<IResult<List<JobTabReviewNoteResponseDto>>> Handle(GetLatestJobTabReviewsQuery request, CancellationToken cancellationToken)
    {
        var result = await jobTabReviewRepository.GetLastReviewCycleAsync(request.JobId);

        if (result.IsFailed)
            return Result.Fail<List<JobTabReviewNoteResponseDto>>(result.Errors);

        var jobTabReview = result.Value;

        if (jobTabReview is null)
            return Result.Fail<List<JobTabReviewNoteResponseDto>>(JobMessages.JobTabReviewNotFound);

        var jobDto = mapper.Map<List<JobTabReviewNoteResponseDto>>(jobTabReview);
        return Result.Ok(jobDto);
    }
}
