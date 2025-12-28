using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetLatestReviewQueryHnadler(IMediator mediator)
    : IRequestHandler<GetLatestReviewQuery, IResult<JobReviewResponseDto>>
{
    public async Task<IResult<JobReviewResponseDto>> Handle(GetLatestReviewQuery request, CancellationToken cancellationToken)
    {
        var tabReviewReponse = await mediator.Send(new GetLatestJobTabReviewsQuery(request.JobId), cancellationToken);
        var reviewAttachmentReponse = await mediator.Send(new GetLatestJobReviewAttachmentQuery(request.JobId), cancellationToken);

        var newJobReviewResp = new JobReviewResponseDto()
        {
            TabNoteReviews = tabReviewReponse.Value,
            ReviewAttachment = reviewAttachmentReponse.Value
        };

        return Result.Ok(newJobReviewResp);
    }
}
