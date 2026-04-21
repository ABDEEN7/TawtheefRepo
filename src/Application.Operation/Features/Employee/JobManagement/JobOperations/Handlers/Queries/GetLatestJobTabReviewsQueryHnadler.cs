using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;
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
            return Result.Fail<List<JobTabReviewNoteResponseDto>>(JobMessages.JobTabReviewNotFound);

        var jobDto = mapper.Map<List<JobTabReviewNoteResponseDto>>(jobTabReview);
        return Result.Ok(jobDto);
    }
}

