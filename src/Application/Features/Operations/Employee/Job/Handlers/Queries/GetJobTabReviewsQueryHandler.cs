using FluentResults;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobTabReviewsQueryHandler(IJobTabReviewNoteRepository jobTabReviewRepository, IMapper mapper)
    : IRequestHandler<GetJobTabReviewsQuery, IResult<List<JobTabReviewNoteResponseDto>>>
{
    public async Task<IResult<List<JobTabReviewNoteResponseDto>>> Handle(GetJobTabReviewsQuery request, CancellationToken cancellationToken)
    {
        var result = await jobTabReviewRepository.GetByIdWithDetailsAsync(request.jobId);

        if (result.IsFailed)
            return Result.Fail<List<JobTabReviewNoteResponseDto>>(result.Errors);

        var job = result.Value;

        if (job is null)
            return Result.Fail<List<JobTabReviewNoteResponseDto>>(JobMessages.JOB_NOT_FOUND);

        var jobDto = mapper.Map<List<JobTabReviewNoteResponseDto>>(job);
        return Result.Ok(jobDto);
    }
}
