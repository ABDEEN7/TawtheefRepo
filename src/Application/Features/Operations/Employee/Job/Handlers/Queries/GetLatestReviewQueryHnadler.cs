using Cortex.Mediator;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetLatestReviewQueryHnadler(IMediator mediator)
    : IQueryHandler<GetLatestReviewQuery, IResult<JobReviewResponseDto>>
{
    public async Task<IResult<JobReviewResponseDto>> Handle(GetLatestReviewQuery request, CancellationToken cancellationToken)
    {
        var tabReviewReponse = await mediator.SendQueryAsync<GetLatestJobTabReviewsQuery, IResult<List<JobTabReviewNoteResponseDto>>>(new GetLatestJobTabReviewsQuery(request.JobId), cancellationToken);
        var reviewAttachmentReponse = await mediator.SendQueryAsync<GetLatestJobReviewAttachmentQuery, IResult<FileRefDto?>>(new GetLatestJobReviewAttachmentQuery(request.JobId), cancellationToken);

        var newJobReviewResp = new JobReviewResponseDto()
        {
            TabNoteReviews = tabReviewReponse.Value,
            ReviewAttachment = reviewAttachmentReponse.Value
        };

        return Result.Ok(newJobReviewResp);
    }
}
