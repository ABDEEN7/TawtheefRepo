using Application.Operation.Features.Employee.Interview.Evaluation.Queries;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Handlers.Queries;

public sealed class GetMySessionQueryHandler(IMediator mediator, EvaluationSessionAccessResolver sessionAccess)
    : IRequestHandler<GetMySessionQuery, IResult<ScheduleDto>>
{
    public async Task<IResult<ScheduleDto>> Handle(GetMySessionQuery request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetScheduleByIdQuery(request.ScheduleId), cancellationToken);
        if (result.IsFailed)
            return result;

        var canAccess = await sessionAccess.CanAccessJobAsync(result.Value.JobId, cancellationToken);
        if (!canAccess)
            return Result.Fail<ScheduleDto>(new Error(ErrorsCodes.InterviewEvaluationSessionAccessDenied)
                .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));

        return result;
    }
}
