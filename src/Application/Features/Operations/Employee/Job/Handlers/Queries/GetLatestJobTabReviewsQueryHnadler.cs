using FluentResults;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;
public class GetLatestJobTabReviewsQueryHandler(IJobTabReviewNoteRepository jobTabReviewRepository, IMapper mapper)
    : IRequestHandler<GetLatestJobTabReviewsQuery, IResult<List<JobTabReviewNoteResponseDto>>>
{
    public async Task<IResult<List<JobTabReviewNoteResponseDto>>> Handle(GetLatestJobTabReviewsQuery request, CancellationToken cancellationToken)
    {
        var result = await jobTabReviewRepository.GetLastReviewCycleAsync(request.JobId);

        if (result.IsFailed)
            return Result.Fail<List<JobTabReviewNoteResponseDto>>(result.Errors);

        var jobTabReview = result.Value;

        if (jobTabReview is null)
            return Result.Fail<List<JobTabReviewNoteResponseDto>>(JobMessages.JOB_TAB_REVIEW_NOT_FOUND);

        var jobDto = mapper.Map<List<JobTabReviewNoteResponseDto>>(jobTabReview);
        return Result.Ok(jobDto);
    }
}
