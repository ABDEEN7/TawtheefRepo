using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;

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


