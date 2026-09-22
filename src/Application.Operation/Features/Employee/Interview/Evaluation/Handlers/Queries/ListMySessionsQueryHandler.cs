using Application.Operation.Features.Employee.Interview.Evaluation.Queries;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Handlers.Queries;

public sealed class ListMySessionsQueryHandler(IMediator mediator, EvaluationSessionAccessResolver sessionAccess)
    : IRequestHandler<ListMySessionsQuery, IResult<List<ScheduleListItemDto>>>
{
    public async Task<IResult<List<ScheduleListItemDto>>> Handle(ListMySessionsQuery request, CancellationToken cancellationToken)
    {
        // Reuses ListSchedulesQueryHandler's own DTO-building/aggregation rather than duplicating it -
        // this handler only adds the membership filter on top.
        var allResult = await mediator.Send(new ListSchedulesQuery(null, null), cancellationToken);
        if (allResult.IsFailed)
            return allResult;

        var accessibleJobIds = await sessionAccess.GetAccessibleJobIdsAsync(cancellationToken);
        if (accessibleJobIds is null)
            return allResult;

        return Result.Ok(allResult.Value.Where(s => accessibleJobIds.Contains(s.JobId)).ToList());
    }
}
