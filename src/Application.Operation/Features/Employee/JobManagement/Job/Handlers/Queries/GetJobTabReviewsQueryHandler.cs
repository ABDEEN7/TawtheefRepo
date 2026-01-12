using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Application.Operation.Features.Employee.JobManagement.Job.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Queries;

public class GetJobTabReviewsQueryHandler(IJobTabReviewNoteRepository jobTabReviewRepository, IMapper mapper)
    : IQueryHandler<GetJobTabReviewsQuery, IResult<List<JobTabReviewNoteResponseDto>>>
{
    public async Task<IResult<List<JobTabReviewNoteResponseDto>>> Handle(GetJobTabReviewsQuery request, CancellationToken cancellationToken)
    {
        var result = await jobTabReviewRepository.GetByIdWithDetailsAsync(request.JobId);

        if (result.IsFailed)
            return Result.Fail<List<JobTabReviewNoteResponseDto>>(result.Errors);

        var job = result.Value;

        if (job is null)
            return Result.Fail<List<JobTabReviewNoteResponseDto>>(JobMessages.JobNotFound);

        var jobDto = mapper.Map<List<JobTabReviewNoteResponseDto>>(job);
        return Result.Ok(jobDto);
    }
}
